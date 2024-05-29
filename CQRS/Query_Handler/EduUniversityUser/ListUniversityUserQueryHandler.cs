using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduAdmin;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Helpers;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduTransaction
{
    public class UniversityUserDTO
    {
        public UniversityUser UniversityUser { get; set; }
        public Role Role { get; set; }
        public List<Department> Departments { get; set; }
        public List<University> Universities { get; set; }
    }

    public class ListUniversityUserQueryHandler : IRequestHandler<ListUniversityUserQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;

        public ListUniversityUserQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<object>> Handle(ListUniversityUserQuery request, CancellationToken cancellationToken)
        {

            var pageSize = request.PaginationUniversityUserDTO.PageSize;
            var pageNumber = request.PaginationUniversityUserDTO.Page;
            var enuUniversityUsers = new List<UniversityUserDTO>();
            var totalCount = _context.UniversityUsers.Count();
            var universityUsers = _context.UniversityUsers.ToList();
            var roles = _context.Roles.ToList();
            var universityDepartmentUsers = _context.UniversityDepartmentUsers.ToList();
            var departments = _context.Departments.ToList();
            var universities = _context.Universities.ToList();

            foreach (var u in universityUsers)
            {
                var userRoles = roles.Where(r => r.Id == u.RoleId).ToList();
                var departmentUsers = universityDepartmentUsers.Where(udu => udu.UniversityUserId == u.Id).ToList();
                var departmentIds = departmentUsers.Select(udu => udu.DepartmentId).ToList();
                var departmentDetails = departments.Where(d => departmentIds.Contains(d.Id)).ToList();
                var universityIds = departmentUsers.Select(udu => udu.UniversityId).ToList();
                var universityDetails = universities.Where(u => universityIds.Contains(u.Id)).ToList();
                var universityUser = new UniversityUserDTO
                {
                    UniversityUser = u,
                    Role = userRoles.FirstOrDefault(),
                    Departments = departmentDetails,
                    Universities = universityDetails
                };
                enuUniversityUsers.Add(universityUser);
            }
            enuUniversityUsers = enuUniversityUsers.ToList();
            IQueryable<UniversityUserDTO> _universityUsers = enuUniversityUsers.AsQueryable();
            if (request.PaginationUniversityUserDTO.DepartmentId != Guid.Empty)
            {
                _universityUsers = _universityUsers.Where(u => u.Departments.Any(d => d.Id == request.PaginationUniversityUserDTO.DepartmentId));
            }
            if (request.PaginationUniversityUserDTO.UniversityId != Guid.Empty)
            {
                _universityUsers = _universityUsers.Where(u => u.Universities.Any(u => u.Id == request.PaginationUniversityUserDTO.UniversityId));
            }
            if (request.PaginationUniversityUserDTO.Guid != Guid.Empty)
            {
                _universityUsers = _universityUsers.Where(u => u.UniversityUser.Id == request.PaginationUniversityUserDTO.Guid);
            }
            if (!string.IsNullOrWhiteSpace(request.PaginationUniversityUserDTO.SearchTerm))
            {
                _universityUsers = _universityUsers.Where(u =>
                    u.UniversityUser.Name.ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower()) ||
                    u.UniversityUser.Email.ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower()));
            }

            var universityList = await PagedList<UniversityUserDTO>.CreateAsync(
                _universityUsers.Select(u => new UniversityUserDTO
                {
                    Role = u.Role,
                    UniversityUser = u.UniversityUser,
                    Departments = u.Departments,
                    Universities = u.Universities
                }),
                request.PaginationUniversityUserDTO.Page,
                request.PaginationUniversityUserDTO.PageSize);

            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { items = _universityUsers, request.PaginationUniversityUserDTO.Page, request.PaginationUniversityUserDTO.PageSize, totalCount, hasNextPage = (request.PaginationUniversityUserDTO.Page * request.PaginationUniversityUserDTO.PageSize < totalCount) }, message: "University list");
        }
    }
}
using System.Linq.Expressions;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduAdmin;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduTransaction
{
    public class NewListUniversityUserQueryHandler : IRequestHandler<NewListUniversityUserQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        public NewListUniversityUserQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<object>> Handle(NewListUniversityUserQuery request, CancellationToken cancellationToken)
        {
            List<Guid> UniversityDepartmentUserGuidList = new List<Guid>();
            List<UniversityUserDTO> enuUniversityUsers = new List<UniversityUserDTO>();

            UniversityDepartmentUserGuidList = _context.UniversityDepartmentUsers
                .Where(udu =>
                    (request.PaginationUniversityUserDTO.DepartmentId == Guid.Empty || udu.DepartmentId == request.PaginationUniversityUserDTO.DepartmentId) &&
                    (request.PaginationUniversityUserDTO.UniversityId == Guid.Empty || udu.UniversityId == request.PaginationUniversityUserDTO.UniversityId) &&
                    (request.PaginationUniversityUserDTO.Guid == Guid.Empty || udu.UniversityUserId == request.PaginationUniversityUserDTO.Guid)
                    )
                .Select( udu => udu.UniversityUserId)
                .Distinct()
                .ToList();

            var universityUsers = _context.UniversityUsers.Where( uu => uu.IsDeleted == false ).ToList();
            var roles = _context.Roles.ToList();
            var universityDepartmentUsers = _context.UniversityDepartmentUsers.ToList();
            var departments = _context.Departments.Where(uu => uu.IsDeleted == false).ToList();
            var universities = _context.Universities.ToList();
            foreach (Guid guid in UniversityDepartmentUserGuidList)
            {
                var universityUser = universityUsers.Where(uu => uu.Id == guid).FirstOrDefault();
                if(universityUser != null)
                {
                    var userRoles = roles.Where(r => r.Id == universityUser.RoleId).ToList();
                    var departmentUsers = universityDepartmentUsers.Where(udu => udu.UniversityUserId == guid).ToList();
                    var departmentIds = departmentUsers.Select(udu => udu.DepartmentId).ToList();
                    var departmentDetails = departments.Where(d => departmentIds.Contains(d.Id)).ToList();
                    var universityIds = departmentUsers.Select(udu => udu.UniversityId).ToList();
                    var universityDetails = universities.Where(u => universityIds.Contains(u.Id)).ToList();
                    UniversityUserDTO universityUserDTO = new UniversityUserDTO
                    {
                        UniversityUser = universityUser,
                        Role = userRoles != null ? userRoles.FirstOrDefault() : null,
                        Departments = departmentDetails,
                        Universities = universityDetails
                    };
                    enuUniversityUsers.Add(universityUserDTO);

                }
            }
            if (!string.IsNullOrWhiteSpace(request.PaginationUniversityUserDTO.SearchTerm))
            {
                enuUniversityUsers = enuUniversityUsers.Where(u =>
                    u.UniversityUser.Name.ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower()) ||
                    u.UniversityUser.Email.ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower())).ToList();
            }
            if (request.PaginationUniversityUserDTO.SortOrder?.ToLower() == "desc")
            {
                enuUniversityUsers = enuUniversityUsers.AsQueryable().OrderByDescending(GetSortProperty(request)).ToList();
            }
            else
            {
                enuUniversityUsers = enuUniversityUsers.AsQueryable().OrderBy(GetSortProperty(request)).ToList();
            }

            int totalCount = enuUniversityUsers.Count();


            enuUniversityUsers = enuUniversityUsers.Skip((request.PaginationUniversityUserDTO.Page - 1) * request.PaginationUniversityUserDTO.PageSize).Take(request.PaginationUniversityUserDTO.PageSize).ToList();

            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { items = enuUniversityUsers, request.PaginationUniversityUserDTO.Page, request.PaginationUniversityUserDTO.PageSize, totalCount, hasNextPage = (request.PaginationUniversityUserDTO.Page * request.PaginationUniversityUserDTO.PageSize < totalCount) }, message: "University list");
        }

        private static Expression<Func<UniversityUserDTO, object>> GetSortProperty(NewListUniversityUserQuery request)
        {
            return request.PaginationUniversityUserDTO.SortColumn?.ToLower() switch
            {
                "Name" => uu => uu.UniversityUser.Name,
                "Email" => uu => uu.UniversityUser.Email,
                "CreatedAt" => uu => uu.UniversityUser.CreatedAt,
                _ => uu => uu.UniversityUser.CreatedAt
            };
        }
    }
}
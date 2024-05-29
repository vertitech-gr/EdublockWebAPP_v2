using System.Linq;
using System.Linq.Expressions;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduDepartment;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler
{
    public class DepartmentQueryByUniversityIdHandler : IRequestHandler<DepartmentQueryByUniversityId, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<Role> _roleRepository;

        public DepartmentQueryByUniversityIdHandler(IRepository<Role> roleRepository, EduBlockDataContext context, IRepository<Department> departmentRepository)
        {
            _context = context;
            _departmentRepository = departmentRepository;
            _roleRepository = roleRepository;
        }

        public async Task<ApiResponse<object>> Handle(DepartmentQueryByUniversityId request, CancellationToken cancellationToken)
        {
            if (request.PaginationGuidDTO.guid == Guid.Empty)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, message: "University Id Required");
            }
            Role role = await _roleRepository.FindAsync(r => r.NormalizedName == Authorization.Role.SECRETARIAT.ToString());

            // Get all departments for the specified university
            var departments = _context.Departments.Where( d => d.IsDeleted == false )
                .Where(department => department.UniversityID == request.PaginationGuidDTO.guid)
                .ToList();

            // Get all related UniversityDepartmentUsers for these departments
            var departmentStudents = _context.DepartmentStudents
                .Where(udu => departments.Select(d => d.Id).Contains(udu.DepartmentId))
                .ToList();

            // Get all related UserRequests for these departments
            var userRequests = _context.UserRequest
                .Where(ur => departmentStudents.Select(d => d.StudentId).Contains(ur.SenderId))
                .ToList();

            // Get all related Users for these departments
            var users = _context.User.Where(d => d.IsDeleted == false)
                .Where(u => departments.Select(d => d.Id).Contains(u.Id))
                .ToList();

            // Get all related UniversityDepartmentUsers for these departments
            var universityDepartmentUsers = _context.UniversityDepartmentUsers
                .Where(udu => departments.Select(d => d.Id).Contains(udu.DepartmentId))
                .ToList();

            // Get all related UniversityUsers for these UniversityDepartmentUsers with the specified role
            var universityUsers = _context.UniversityUsers.Where(d => d.IsDeleted == false)
                .Where(uu => universityDepartmentUsers.Select(udu => udu.UniversityUserId).Contains(uu.Id) && uu.RoleId == role.Id)
                .ToList();

            // departmentStudents.Where(ds => ds.DepartmentId == Guid.Parse("40e4b5a7-76d3-4f1e-a79b-5ab9ab307971");

            var requestsss = userRequests.Where(ur => departmentStudents.Where(ds => ds.DepartmentId == Guid.Parse( "40e4b5a7-76d3-4f1e-a79b-5ab9ab307971" ) ).Select(ds => ds.StudentId).Contains(ur.SenderId)).Count();
                                                 

            var departmentResponseQuery = (from department in departments
                                            join ur in userRequests on department.Id equals ur.SenderId into dr
                                           from ur in dr.DefaultIfEmpty()
                                           join user in users on department.Id equals user.Id into du
                                           from user in du.DefaultIfEmpty()
                                           join udu in universityDepartmentUsers on department.Id equals udu.DepartmentId into dudu
                                           from udu in dudu.DefaultIfEmpty()
                                           join uu in universityUsers on udu?.UniversityUserId equals uu.Id into duuu
                                           from uu in duuu.DefaultIfEmpty()
                                              select new DepartmentResponseDTO
                                              {
                                                  Id = department.Id,
                                                  Name = department.Name,
                                                  CreatedAt = department.CreatedAt,
                                                  Email = department.Email,
                                                  loginStatus = department.loginStatus,
                                                  Status = department.Status,
                                                  Type = department.Type,
                                                  UniversityID = department.UniversityID,
                                                  requests = userRequests.Where( ur =>  departmentStudents.Where(ds => ds.DepartmentId == department.Id).Select(ds => ds.StudentId).Contains( ur.SenderId )  ).Count(),
                                                  students = departmentStudents.Where(ds => ds.DepartmentId == department.Id).Select( ds => ds.StudentId).Count(),
                                                  userRequest = ur,
                                                  user = user,
                                                  universityUser = uu,
                                              })
                                 .GroupBy(drd => drd.Id)
                                 .SelectMany(g =>
                                 {
                                     // Filter out null universityUser items and select the first non-null item
                                     var nonNullItems = g.Where(drd => drd.universityUser != null).ToList();
                                     if (nonNullItems.Any())
                                     {
                                         // If there are non-null items, return them
                                         return nonNullItems;
                                     }
                                     else
                                     {
                                         // Otherwise, return all items (including null universityUser items)
                                         return g.ToList();
                                     }
                                 })
                                .ToList();

            IQueryable<DepartmentResponseDTO> departmentResponseDTOs = departmentResponseQuery.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.PaginationGuidDTO.SearchTerm))
            {
                departmentResponseDTOs = departmentResponseDTOs.Where(p =>
                p.Type.ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
                 p.universityUser != null &&
                    p.universityUser.Name.ToLower().Contains(request.PaginationGuidDTO.SearchTerm) || p.universityUser != null &&
                    ((string)p.universityUser.Email).ToLower().Contains(request.PaginationGuidDTO.SearchTerm));
            }

            if (request.PaginationGuidDTO.SortOrder?.ToLower() == "desc")
            {
                departmentResponseDTOs = departmentResponseDTOs.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                departmentResponseDTOs = departmentResponseDTOs.OrderBy(GetSortProperty(request));
            }

            var totalCount = departmentResponseDTOs.Count();
            departmentResponseDTOs = departmentResponseDTOs.Skip((request.PaginationGuidDTO.Page - 1) * request.PaginationGuidDTO.PageSize).Take(request.PaginationGuidDTO.PageSize);
            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { items = departmentResponseDTOs, request.PaginationGuidDTO.Page, request.PaginationGuidDTO.PageSize, totalCount, hasNextPage = (request.PaginationGuidDTO.Page * request.PaginationGuidDTO.PageSize < totalCount), hasPreviousPage = (request.PaginationGuidDTO.Page > 1) }, message: "Department list");
        }

        private static Expression<Func<DepartmentResponseDTO, object>> GetSortProperty(DepartmentQueryByUniversityId request)
        {
            return request.PaginationGuidDTO.SortColumn?.ToLower() switch
            {
                "name" => employer => employer.Name,
                "email" => employer => employer.Email,
                "CreatedAt" => employer => employer.CreatedAt,
                _ => employer => employer.CreatedAt
            };
        }
    }
}



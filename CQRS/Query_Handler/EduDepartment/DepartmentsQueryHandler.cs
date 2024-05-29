using System.Linq.Expressions;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduDepartment;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Helpers;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler
{
    public class DepartmentsQueryHandler : IRequestHandler<DepartmentQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<Role> _roleRepository;

        public DepartmentsQueryHandler(EduBlockDataContext context, IRepository<Role> roleRepository, IRepository<Department> departmentRepository)
        {
            _context = context;
            _roleRepository = roleRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<ApiResponse<object>> Handle(DepartmentQuery request, CancellationToken cancellationToken)
        {

            var departmentResponseQuery = _context.Departments.Where(d => d.Id == request.PaginationUniversityGuidDTO.guid)
            .GroupJoin(
                _context.UserRequest,
                department => department.Id,
                userRequest => userRequest.SenderId,
                (department, departmentUserRequests) => new { department, departmentUserRequests }
            )
            .SelectMany(
                x => x.departmentUserRequests.DefaultIfEmpty(),
                (x, du) => new { x.department, du }
            )
            .GroupJoin(
                _context.User,
                x => x.department.Id,
                user => user.Id,
                (x, departmentUsers) => new { x.department, x.du, departmentUsers }
            )
            .SelectMany(
                x => x.departmentUsers.DefaultIfEmpty(),
                (x, u) => new { x.department, x.du, u }
            )
            .GroupJoin(
                _context.UniversityDepartmentUsers,
                x => x.department.Id,
                udu => udu.DepartmentId,
                (x, universityDepartmentUsers) => new { x.department, x.du, x.u, universityDepartmentUsers }
            )
            .SelectMany(
                x => x.universityDepartmentUsers.DefaultIfEmpty(),
                (x, udu) => new { x.department, x.du, x.u, udu }
            )
            .GroupJoin(
                _context.UniversityUsers,
                x => x.udu.UniversityUserId,
                uu => uu.Id,
                (x, universityUsers) => new { x.department, x.du, x.u, x.udu, universityUsers }
            )
            .SelectMany(
                x => x.universityUsers.DefaultIfEmpty(),
                (x, uu) => new DepartmentResponseDTO
                {
                    Id = x.department.Id,
                    Name = x.department.Name,
                    CreatedAt = x.department.CreatedAt,
                    Email = x.department.Email,
                    loginStatus = x.department.loginStatus,
                    Status = x.department.Status,
                    Type = x.department.Type,
                    UniversityID = x.department.UniversityID,
                    userRequest = x.du,
                    user = x.u,
                    universityUser = uu
                }
            );

            if (request.PaginationUniversityGuidDTO.guid != Guid.Empty)
            {
                departmentResponseQuery = departmentResponseQuery.Where(department => department.Id == request.PaginationUniversityGuidDTO.guid);
            }

            if (request.PaginationUniversityGuidDTO.UniversityID != Guid.Empty)
            {
                departmentResponseQuery = departmentResponseQuery.Where(department => department.UniversityID == request.PaginationUniversityGuidDTO.UniversityID);
            }

            IQueryable<DepartmentResponseDTO> departmentResponseDTOs = departmentResponseQuery.AsQueryable();
            Role role = await _roleRepository.FindAsync(r => r.NormalizedName == Authorization.Role.SECRETARIAT.ToString());
            departmentResponseDTOs = departmentResponseDTOs.Where(drd => (drd.universityUser == null || drd.universityUser.RoleId == role.Id));


            if (!string.IsNullOrWhiteSpace(request.PaginationUniversityGuidDTO.SearchTerm))
            {
                departmentResponseDTOs = departmentResponseDTOs.Where(p =>
                    p.Name.ToLower().Contains(request.PaginationUniversityGuidDTO.SearchTerm) ||
                    ((string)p.Email).ToLower().Contains(request.PaginationUniversityGuidDTO.SearchTerm));
            }
            if (request.PaginationUniversityGuidDTO.SortOrder?.ToLower() == "desc")
            {
                departmentResponseDTOs = departmentResponseDTOs.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                departmentResponseDTOs = departmentResponseDTOs.OrderBy(GetSortProperty(request));
            }

            var departmentResponseList = await PagedList<DepartmentResponseDTO>.CreateAsync(
                            departmentResponseDTOs,
                            request.PaginationUniversityGuidDTO.Page,
            request.PaginationUniversityGuidDTO.PageSize);

            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: departmentResponseList, message: "Department list");
        }

        private static Expression<Func<DepartmentResponseDTO, object>> GetSortProperty(DepartmentQuery request)
        {
            return request.PaginationUniversityGuidDTO.SortColumn?.ToLower() switch
            {
                "name" => employer => employer.Name,
                "email" => employer => employer.Email,
                "CreatedAt" => employer => employer.CreatedAt,
                _ => employer => employer.CreatedAt
            };
        }
    }
}



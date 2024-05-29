using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduDepartment;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduDepartment
{
    public class DepartmentQueryByUniversityUserIdHandler : IRequestHandler<DepartmentQueryByUniversityUserId, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;


        public DepartmentQueryByUniversityUserIdHandler(EduBlockDataContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<object>> Handle(DepartmentQueryByUniversityUserId request, CancellationToken cancellationToken)
        {
            var universityUsersDTOs = (
                from uu in _context.UniversityUsers
                where uu.Id == request._uniqueId
                join r in _context.Roles on uu.RoleId equals r.Id into roleJoin
                from role in roleJoin.DefaultIfEmpty()
                join udu in _context.UniversityDepartmentUsers on uu.Id equals udu.UniversityUserId into universityUserJoin
                from universtyDepartmentUser in universityUserJoin.DefaultIfEmpty()
                join d in _context.Departments on universtyDepartmentUser.DepartmentId equals d.Id into departmentJoin
                from department in departmentJoin.DefaultIfEmpty()
                join u in _context.Universities on universtyDepartmentUser.UniversityId equals u.Id into universityJoin
                from university in universityJoin.DefaultIfEmpty()
                select new UniversityUsersDTO
                {
                    Role = role,
                    UniversityUser = uu,
                    Department = department,
                    University = university
                }).ToList();




            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: universityUsersDTOs, message: "Department list According To University User");
        }
    }
}


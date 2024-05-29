using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduDepartment
{
    public class DepartmentQuery : IRequest<ApiResponse<object>>
    {
        public CommanUser User { get; set; }
        public PaginationUniversityGuidDTO PaginationUniversityGuidDTO { get; set; }

        public DepartmentQuery(CommanUser user, PaginationUniversityGuidDTO paginationUniversityGuidDTO)
        {
            User = user;
            PaginationUniversityGuidDTO = paginationUniversityGuidDTO;
        }
    }

    public class DepartmentQueryByUniversityId : IRequest<ApiResponse<object>>
    {
        public PaginationGuidDTO PaginationGuidDTO { get; set; }
        public DepartmentQueryByUniversityId(PaginationGuidDTO paginationGuidDTO)
        {
            PaginationGuidDTO = paginationGuidDTO;
        }
    }
    public class DepartmentQueryByUniversityUserId : IRequest<ApiResponse<object>>
    {
        public Guid _uniqueId { get; set; }
        public DepartmentQueryByUniversityUserId(Guid uniqueId)
        {
            _uniqueId = uniqueId;
        }
    }
}
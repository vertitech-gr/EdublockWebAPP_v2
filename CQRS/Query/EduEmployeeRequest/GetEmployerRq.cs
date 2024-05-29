using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduEmployeeRequest
{
    public class GetEmployerRq : IRequest<ApiResponse<object>>
    {
        public CommanUser User { get; set; }
        public PaginationStatusGuidDTO PaginationStatusGuidDTO { get; set; }
        public GetEmployerRq(CommanUser user, PaginationStatusGuidDTO paginationStatusGuidDTO)
        {
            User = user;
            PaginationStatusGuidDTO = paginationStatusGuidDTO;
        }
    }
}
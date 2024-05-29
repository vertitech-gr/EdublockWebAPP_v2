using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduEmployer
{
    public class GetEmployerListQuery : IRequest<ApiResponse<object>>
    {
        public CommanUser User { get; set; }
        public PaginationDTO PaginationDTO { get; set; }

        public GetEmployerListQuery(CommanUser user, PaginationDTO paginationDTO) {
            User = user;
            PaginationDTO  = paginationDTO;
        }
    }

    public class GetEmployerTokenListQuery : IRequest<ApiResponse<object>>
    {
        public CommanUser User { get; set; }
        public PaginationGuidDTO PaginationDTO { get; set; }

        public GetEmployerTokenListQuery(CommanUser user, PaginationGuidDTO paginationDTO)
        {
            User = user;
            PaginationDTO = paginationDTO;
        }
    }
}

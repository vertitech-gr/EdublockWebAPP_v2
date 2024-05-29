using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduUser
{
    public class GetPaymentTransactionQuery : IRequest<ApiResponse<object>>
    {
        public PaginationGuidDTO PaginationGuidDTO;
        public GetPaymentTransactionQuery(PaginationGuidDTO paginationGuidDTO)
        {
            PaginationGuidDTO = paginationGuidDTO;
        }
    }
}

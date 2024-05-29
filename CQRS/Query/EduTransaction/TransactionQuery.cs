using Edu_Block.DAL.EF;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduTransaction
{
    public class TransactionQuery : IRequest<List<TransactionResponseDTO>>
    {
        public CommanUser User { get; set; }
        public TransactionQuery(CommanUser user)
        {
            User = user;
        }
    }

    public class AllTransactionQuery : IRequest<ApiResponse<object>>
    {
        public CommanUser User { get; set; }
        public PaginationGuidDTO TransactionPaginationDTO { get; set; }

        public AllTransactionQuery(CommanUser user, PaginationGuidDTO transactionPaginationDTO)
        {
            User = user;
            TransactionPaginationDTO = transactionPaginationDTO;
        }
    }

    public class GetBalanceQuery : IRequest<BalanceDTO>
    {
        public CommanUser User { get; set; }
        public GetBalanceQuery(CommanUser user)
        {
            User = user;
        }
    }

   

}

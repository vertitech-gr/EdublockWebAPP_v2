using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduTransaction
{
    public class TransactionCommand : IRequest<Transactions>
    {
        public TransactionDTO transactionDTO { get; set; }
        public CommanUser User { get; set; }

        public TransactionCommand(TransactionDTO _transactionDTO, CommanUser user)
        {
            this.transactionDTO = _transactionDTO;
            User = user;
        }
    }
}

using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduPurchaseSubscription
{
    public class PurchaseSubscriptionCommand : IRequest<PurchaseSubscription>
    {
        public PurchaseSubscriptionDTO purchaseSubscriptionDto { get; set; }
        public CommanUser? User { get; set; }

        public PurchaseSubscriptionCommand(PurchaseSubscriptionDTO _purchaseSubscriptionDto, CommanUser user)
        {
            purchaseSubscriptionDto = _purchaseSubscriptionDto;
            User = user;
        }
    }

    public class PurchaseCoinsCommand : IRequest<Transactions>
    {
        public PurchaseCoinsDTO PurchaseCoinsDTO { get; set; }
        public CommanUser User { get; set; }
        public TransactionType TransactionType { get; set; }

        public PurchaseCoinsCommand(PurchaseCoinsDTO purchaseCoinsDTO,  CommanUser user, TransactionType transactionType = TransactionType.Debit)
        {
            PurchaseCoinsDTO = purchaseCoinsDTO;
            User = user;
            TransactionType = transactionType;
        }
    }
}

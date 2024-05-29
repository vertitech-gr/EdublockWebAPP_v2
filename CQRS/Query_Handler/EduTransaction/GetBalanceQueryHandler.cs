using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduPurchaseSubscription;
using Edu_Block_dev.CQRS.Query.EduTransaction;
using Microsoft.EntityFrameworkCore;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Edu_Block.DAL;

namespace Edu_Block_dev.CQRS.Query_Handler.EduTransaction
{
    public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, BalanceDTO>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMediator _mediator;
        private readonly IRepository<AvailableSubscription> _avaiableSubscriptionRepository;

        public GetBalanceQueryHandler(EduBlockDataContext context, IMediator mediator, IRepository<AvailableSubscription> avaiableSubscriptionRepository)
        {
            _context = context;
            _mediator = mediator;
            _avaiableSubscriptionRepository = avaiableSubscriptionRepository;
        }

        public async Task<BalanceDTO> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            CommanUser employer = request.User;
            List<PurchaseSubscriptionResponseDTO> enuSubscriptionsList = await _mediator.Send(new PurchaseSubscriptionQuery(employer.Id));
            IQueryable<PurchaseSubscriptionResponseDTO> subscriptionsList = enuSubscriptionsList.AsQueryable();
            PurchaseSubscriptionResponseDTO subscription = subscriptionsList.Where(sub => !sub.is_deleted)
            .OrderByDescending(sub => sub.CreatedAt)
            .FirstOrDefault();

            decimal totalCoinsInSubscriptions = subscriptionsList.Where(p => p.UserProfileID == employer.UserProfile.Id)
                .Sum(p => p.CoinBalance);

            var totalAmountFromTransactions = await _context.Transaction
                 .Where(t => t.UserProfileID == employer.UserProfile.Id)
                 .SumAsync(t => t.Amount, cancellationToken);

            AvailableSubscription availableSubscription = null;

            if (subscription != null)
            {
                 availableSubscription = await _avaiableSubscriptionRepository.FindAsync(u => u.ID == subscription.AvailableSubscriptionID);
            }

            decimal balance = totalCoinsInSubscriptions - totalAmountFromTransactions;
            BalanceDTO balanceDTO = new BalanceDTO();
            balanceDTO.AvailableSubscription = availableSubscription;
            balanceDTO.PurchaseSubscription = subscription;
            balanceDTO.Amount = balance;
            return balanceDTO;
        }
    }
}
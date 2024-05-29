using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduPurchaseSubscription;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Command_Handler.EduPurchaseSubscription
{
    public class PurchaseSubscriptionCommandHandler : IRequestHandler<PurchaseSubscriptionCommand, PurchaseSubscription>
    {
        private readonly EduBlockDataContext _context;

        public PurchaseSubscriptionCommandHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<PurchaseSubscription> Handle(PurchaseSubscriptionCommand request, CancellationToken cancellationToken)
        {
            CommanUser employer = request.User;

            var availableSubscriptionInfo = await _context.AvailableSubscriptions
                .Where(a => a.ID == request.purchaseSubscriptionDto.AvailableSubscriptionID)
            .Select(s => new { s.Coins })
            .FirstOrDefaultAsync();


            var purchaseSubscription = new PurchaseSubscription
            {
                AvailableSubscriptionID = request.purchaseSubscriptionDto.AvailableSubscriptionID,
                CoinBalance = availableSubscriptionInfo.Coins,
                UserProfileID = employer.UserProfile.Id,
                EndDate = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow
            };

            _context.PurchaseSubscriptions.Add(purchaseSubscription);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving AvailableSubscription: {ex.Message}");
                throw;
            }

            return purchaseSubscription;
        }

    }
}

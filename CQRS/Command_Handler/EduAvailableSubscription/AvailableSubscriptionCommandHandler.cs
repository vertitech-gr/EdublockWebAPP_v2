using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduAvailableSubscription;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Command_Handler.EduAvailableSubscription
{
    public class AvailableSubscriptionCommandHandler : IRequestHandler<AvailableSubscriptionCommand, AvailableSubscription>
    {
        private readonly EduBlockDataContext _context;

        public AvailableSubscriptionCommandHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<AvailableSubscription> Handle(AvailableSubscriptionCommand request, CancellationToken cancellationToken)
        {
            CommanUser admin = request.User;
            var availableSubscription = new AvailableSubscription
            {                                               
                Name = request.availableSubscriptionDto.Name,
                Discription = request.availableSubscriptionDto.Discription,
                Type = request.availableSubscriptionDto.Type,
                Coins = request.availableSubscriptionDto.Coins,
                Amount = request.availableSubscriptionDto.Amount,
                created_by = admin.UserProfile.Id,

            };
            _context.AvailableSubscriptions.Add(availableSubscription);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving AvailableSubscription: {ex.Message}");
                throw;
            }
            return availableSubscription;
        }
    }
}

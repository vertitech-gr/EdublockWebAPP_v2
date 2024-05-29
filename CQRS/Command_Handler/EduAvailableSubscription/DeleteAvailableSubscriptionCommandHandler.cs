using System.Net;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduAvailableSubscription;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduAvailableSubscription
{
    public class DeleteAvailableSubscriptionCommandHandler : IRequestHandler<DeleteAvailableSubscriptionCommand, ApiResponse<object>>
    {
        private readonly IRepository<AvailableSubscription> _availableSubscriptionRepository;
        private readonly IRepository<PurchaseSubscription> _purchaseSubscriptionRepository;

        private readonly EduBlockDataContext _context;
        public DeleteAvailableSubscriptionCommandHandler(EduBlockDataContext context, IRepository<PurchaseSubscription> purchaseSubscriptionRepository, IRepository<AvailableSubscription> availableSubscriptionRepository)
        {
            _availableSubscriptionRepository = availableSubscriptionRepository;
            _purchaseSubscriptionRepository = purchaseSubscriptionRepository;
            _context = context;
        }

        public async Task<ApiResponse<object>> Handle(DeleteAvailableSubscriptionCommand request, CancellationToken cancellationToken)
        {
            try {
                CommanUser admin = request.User;

                PurchaseSubscription purchaseSubscription = await _purchaseSubscriptionRepository.FindAsync(a => a.AvailableSubscriptionID == request.Guid);

                if(purchaseSubscription  != null)
                {
                    return new ApiResponse<object>(HttpStatusCode.BadRequest, message: "This subscription is already subscribed. So can't delete this subscription");
                }

                AvailableSubscription availableSubscription = await _availableSubscriptionRepository.FindAsync(a => a.ID == request.Guid);
                if (availableSubscription == null || availableSubscription.is_deleted)
                {
                    return new ApiResponse<object>(HttpStatusCode.BadRequest, message: "invalid request or not found");
                }
                DateTime currentDate = DateTime.UtcNow;
                int count = _context.PurchaseSubscriptions
                                .Where(ps => ps.AvailableSubscriptionID == availableSubscription.ID &&
                                ps.EndDate >= currentDate)
                                .Count();
                if (count > 0)
                {
                    return new ApiResponse<object>(HttpStatusCode.BadRequest, data: count, message: "Subsbcription Active For Some Users");
                }
                availableSubscription.is_deleted = true;
                availableSubscription.is_deleted_at = currentDate;
                await _availableSubscriptionRepository.UpdateAsync(availableSubscription.ID, availableSubscription);
                return new ApiResponse<object>(HttpStatusCode.OK, message: "Subcription Deleted Successfully");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(HttpStatusCode.NotFound, message: "Unable to delete Subcription");
            }
        }
    }
}
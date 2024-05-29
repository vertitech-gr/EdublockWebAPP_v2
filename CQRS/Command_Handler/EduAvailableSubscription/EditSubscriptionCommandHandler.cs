using System.Net;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduAvailableSubscription;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduAvailableSubscription
{
    public class EditSubscriptionCommandHandler : IRequestHandler<EditSubscriptionCommand, ApiResponse<object>>
    {
        private readonly IRepository<AvailableSubscription> _availableSubscriptionRepository;
        private readonly IRepository<PurchaseSubscription> _purchaseSubscriptionRepository;


        public EditSubscriptionCommandHandler(IRepository<AvailableSubscription> availableSubscriptionRepository, IRepository<PurchaseSubscription> purchaseSubscriptionRepository)
        {
            _availableSubscriptionRepository = availableSubscriptionRepository;
            _purchaseSubscriptionRepository = purchaseSubscriptionRepository;

        }

        public async Task<ApiResponse<object>> Handle(EditSubscriptionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                CommanUser admin = request._user;
                var existingAvailableSubscription = await _availableSubscriptionRepository.FindAsync(a => a.ID == request._editSubscriptionDto.Id && a.is_deleted == false);
                PurchaseSubscription purchaseSubscription = await _purchaseSubscriptionRepository.FindAsync(a => a.AvailableSubscriptionID == request._editSubscriptionDto.Id);

                if (purchaseSubscription != null)
                {
                    return new ApiResponse<object>(HttpStatusCode.BadRequest, message: "This subscription is already subscribed. So can't edit this subscription");
                }

                if (existingAvailableSubscription == null)
                {
                    return new ApiResponse<object>(HttpStatusCode.BadRequest, message: "Subscription Not Found");
                }
                existingAvailableSubscription.Name = request._editSubscriptionDto.Name;
                existingAvailableSubscription.Discription = request._editSubscriptionDto.Discription;
                existingAvailableSubscription.Type = request._editSubscriptionDto.Type;
                existingAvailableSubscription.Coins = request._editSubscriptionDto.Coins;
                existingAvailableSubscription.Amount = request._editSubscriptionDto.Amount;
                existingAvailableSubscription.created_by = admin.UserProfile.Id;

                await _availableSubscriptionRepository.UpdateAsync(existingAvailableSubscription.ID, existingAvailableSubscription);
                return new ApiResponse<object>(HttpStatusCode.OK, data: existingAvailableSubscription,message: "Subcription Updated Successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(HttpStatusCode.NotFound, message: "Unable to update Subcription");
            }
        }
    }
}
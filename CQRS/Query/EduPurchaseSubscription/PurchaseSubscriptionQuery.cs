using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduPurchaseSubscription
{
    public class PurchaseSubscriptionQuery : IRequest<List<PurchaseSubscriptionResponseDTO>>
    {
        public Guid Guid { get; set; }
        public PurchaseSubscriptionQuery(Guid guid)
        {
            Guid = guid;
            
        }
    }

    public class PaginationPurchaseSubscriptionQuery : IRequest<ApiResponse<object>>
    {
        public PaginationGuidDTO PaginationGuidDTO;
        public PaginationPurchaseSubscriptionQuery(PaginationGuidDTO paginationGuidDTO)
        {
            PaginationGuidDTO = paginationGuidDTO;
        }
    }
}

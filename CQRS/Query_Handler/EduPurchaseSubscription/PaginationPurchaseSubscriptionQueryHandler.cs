using System.Linq.Expressions;
using Edu_Block_dev.CQRS.Query.EduPurchaseSubscription;
using Edu_Block_dev.Helpers;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduPurchaseSubscription
{
    public class PaginationPurchaseSubscriptionQueryHandler : IRequestHandler<PaginationPurchaseSubscriptionQuery, ApiResponse<object>>
    {
        private readonly IMediator _mediator;

        public PaginationPurchaseSubscriptionQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ApiResponse<object>> Handle(PaginationPurchaseSubscriptionQuery request, CancellationToken cancellationToken)
        {
            List<PurchaseSubscriptionResponseDTO> enuPurchaseSubscriptions = await _mediator.Send(new PurchaseSubscriptionQuery(request.PaginationGuidDTO.guid));

            if (enuPurchaseSubscriptions != null && enuPurchaseSubscriptions.Count() > 0)
            {
                IQueryable<PurchaseSubscriptionResponseDTO> purchaseSubscriptions = enuPurchaseSubscriptions.AsQueryable<PurchaseSubscriptionResponseDTO>();
                if (!string.IsNullOrWhiteSpace(request.PaginationGuidDTO.SearchTerm))
                {
                }
                if (request.PaginationGuidDTO.SortOrder?.ToLower() == "desc")
                {
                    purchaseSubscriptions = purchaseSubscriptions.OrderByDescending(GetSortProperty(request));
                }
                else
                {
                    purchaseSubscriptions = purchaseSubscriptions.OrderBy(GetSortProperty(request));
                }

                var purchaseSubscriptionList = await PagedList<PurchaseSubscriptionResponseDTO>.CreateAsync(
                    purchaseSubscriptions,
                    request.PaginationGuidDTO.Page,
                    request.PaginationGuidDTO.PageSize);

                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: purchaseSubscriptionList, message: "Purchase subscription list");

            }
            else
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new List<PurchaseSubscriptionResponseDTO>(), message: "Purchase subscription list");
            }
        }

        private static Expression<Func<PurchaseSubscriptionResponseDTO, object>> GetSortProperty(PaginationPurchaseSubscriptionQuery request)
        {
            return request.PaginationGuidDTO.SortColumn?.ToLower() switch
            {
                "Name" => purchaseSubscription => purchaseSubscription.Name,
                "StartDate" => purchaseSubscription => purchaseSubscription.StartDate,
                "EndDate" => purchaseSubscription => purchaseSubscription.EndDate,
                "CoinBalance" => purchaseSubscription => purchaseSubscription.CoinBalance,
                "CreatedAt" => purchaseSubscription => purchaseSubscription.CreatedAt,
                _ => purchaseSubscription => purchaseSubscription.CreatedAt
            };
        }
    }
}

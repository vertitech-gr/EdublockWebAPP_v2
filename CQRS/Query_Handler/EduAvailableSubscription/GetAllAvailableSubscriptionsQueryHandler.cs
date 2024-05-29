using System.Linq.Expressions;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduAvailableSubscription;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Helpers;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduAvailableSubscription
{
    public class GetAllAvailableSubscriptionsQueryHandler : IRequestHandler<GetAllAvailableSubscriptionsQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        public GetAllAvailableSubscriptionsQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<object>> Handle(GetAllAvailableSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var enuAvailableSubscriptions = await _context.AvailableSubscriptions
                    .Where(subscription => subscription.is_deleted == false)
                    .OrderBy(subscription => subscription.CreatedAt)
                    .ToListAsync(cancellationToken);
                IEnumerable<AvailableSubscription> availableSubscriptions = enuAvailableSubscriptions.AsEnumerable();
                if (request.PaginationGuidDTO.guid != Guid.Empty)
                {
                    availableSubscriptions = availableSubscriptions.Where(avl => avl.ID == request.PaginationGuidDTO.guid);
                }
                IQueryable<AvailableSubscription> availableSubscription = availableSubscriptions.AsQueryable();
                if (!string.IsNullOrWhiteSpace(request.PaginationGuidDTO.SearchTerm))
                {

                    availableSubscription = availableSubscription.Where(p =>
    p.Name.ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
    p.Coins.ToString().ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
    p.Amount.ToString().ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
    p.CreatedAt.ToString().ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
    (p.Type == SubscriptionType.OneTime && request.PaginationGuidDTO.SearchTerm.ToLower() == "one time") ||
    (p.Type == SubscriptionType.Recurring && request.PaginationGuidDTO.SearchTerm.ToLower() == "recurring")
);

                }
                if (request.PaginationGuidDTO.SortOrder?.ToLower() == "desc")
                {
                    availableSubscription = availableSubscription.OrderByDescending(GetSortProperty(request));
                }
                else
                {
                    availableSubscription = availableSubscription.OrderByDescending(GetSortProperty(request));
                }
                var availableSubscriptionList = await PagedList<AvailableSubscription>.CreateAsync(
                                availableSubscription,
                                request.PaginationGuidDTO.Page,
                request.PaginationGuidDTO.PageSize);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: availableSubscriptionList, message: "Avaiable subscription list");
            }catch(Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, data: null, message: "Unable to fetch avaiable subscription list");

            }
        }

        private static Expression<Func<AvailableSubscription, object>> GetSortProperty(GetAllAvailableSubscriptionsQuery request)
        {
            return request.PaginationGuidDTO.SortColumn?.ToLower() switch
            {
                "name" => avl => avl.Name,
                "coins" => avl => avl.Coins,
                "amount" => avl => avl.Amount,
                "type" => avl => avl.Type,
                "createdAt" => avl => avl.CreatedAt,
                _ => avl => avl.CreatedAt
            };
        }

    }
}

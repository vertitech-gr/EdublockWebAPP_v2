using Edu_Block_dev.DAL.EF;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduAvailableSubscription
{
    public class GetAllAvailableSubscriptionsQuery : IRequest<ApiResponse<object>>
    {
        public PaginationGuidDTO PaginationGuidDTO;
        public GetAllAvailableSubscriptionsQuery(PaginationGuidDTO paginationGuidDTO)
        {
            PaginationGuidDTO = paginationGuidDTO;
        }

    }

    public class GetAvailableSubscriptionsByIdQuery : IRequest<AvailableSubscription>
    {
        public Guid Id { get; set; }
        public GetAvailableSubscriptionsByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}

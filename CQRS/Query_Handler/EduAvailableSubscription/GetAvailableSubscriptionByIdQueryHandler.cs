using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduAvailableSubscription;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduAvailableSubscription
{
    public class GetAvailableSubscriptionByIdQueryHandler : IRequestHandler<GetAvailableSubscriptionsByIdQuery, AvailableSubscription>
    {
        private readonly EduBlockDataContext _context;

        public GetAvailableSubscriptionByIdQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<AvailableSubscription> Handle(GetAvailableSubscriptionsByIdQuery request, CancellationToken cancellationToken)
        {
            var availableSubscriptions = _context.AvailableSubscriptions
                .Where(s => s.ID == request.Id && s.is_deleted == false)
                .OrderBy(subscription => subscription.CreatedAt).SingleOrDefault();
            return availableSubscriptions;
        }
    }
}

using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduPurchaseSubscription
{
    public class GetRoleQuery : IRequest<ApiResponse<object>>
    {
        public Guid Guid { get; set; }
        public CommanUser User { get; set; }
        public GetRoleQuery(Guid guid,CommanUser user)
        {
            Guid = guid;
            User = user;
        }
    }
}
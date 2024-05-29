using Edu_Block.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduPurchaseSubscription
{
    public class GetPermissionQuery : IRequest<ApiResponse<object>>
    {
        public Guid Guid { get; set; }
        public CommanUser User { get; set; }
        public GetPermissionQuery(Guid guid,CommanUser user)
        {
            Guid = guid;
            User = user;
        }
    }
}
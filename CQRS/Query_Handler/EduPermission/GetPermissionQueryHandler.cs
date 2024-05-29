using System.Net;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Query.EduPurchaseSubscription;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduPurchaseSubscription
{
    public class GetPermissionQueryHandler : IRequestHandler<GetPermissionQuery, ApiResponse<object>>
    {
        private readonly IRepository<PermissionDetail> _permissionDetialRepository;
        public GetPermissionQueryHandler(IRepository<PermissionDetail> permissionDetialRepository)
        {
            _permissionDetialRepository = permissionDetialRepository;
        }
        public async Task<ApiResponse<object>> Handle(GetPermissionQuery request, CancellationToken cancellationToken)
        {
            try {
                IEnumerable<PermissionDetail> permissions;
                if (request.Guid == Guid.Empty) {
                    permissions = await _permissionDetialRepository.GetAllAsync();
                }
                else
                {
                    permissions = await _permissionDetialRepository.FindAllAsync( r => r.Id == request.Guid);
                }
                return new ApiResponse<object>(HttpStatusCode.OK, data: new { permissions }, message: "Fetch permissions successfully");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "unable to permissions roles");

            }
        }
    }
}
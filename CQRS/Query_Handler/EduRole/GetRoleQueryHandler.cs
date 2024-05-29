using System.Net;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduPurchaseSubscription;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduPurchaseSubscription
{
    public class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, ApiResponse<object>>
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<RolePermissionMapping> _rolePermissionMapingRepository;
        private readonly EduBlockDataContext _context;

        public GetRoleQueryHandler(EduBlockDataContext context, IRepository<Role> roleRepository, IRepository<RolePermissionMapping> rolePermissionMapingRepository)
        {
            _context = context;
            _roleRepository = roleRepository;
            _rolePermissionMapingRepository = rolePermissionMapingRepository;
        }
        public async Task<ApiResponse<object>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Role> enuRoles;
                if (request.Guid == Guid.Empty)
                {
                    enuRoles = await _roleRepository.GetAllAsync();
                }
                else
                {
                    enuRoles = await _roleRepository.FindAllAsync(r => r.Id == request.Guid);
                }

                IQueryable<Role> roles = enuRoles.AsQueryable<Role>();

                var rolePermissions = roles
      .Select(role => new
      {
          Role = role,
          Permissions = _context.RolePermissionMappings
              .Where(mapping => mapping.RoleId == role.Id)
              .Select(mapping => mapping.PermissionDetail)
              .ToList()
      })
      .ToList();



                return new ApiResponse<object>(HttpStatusCode.OK, data: new { rolePermissions } , message: "Fetch roles successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "unable to fetch roles");

            }
        }
    }
}
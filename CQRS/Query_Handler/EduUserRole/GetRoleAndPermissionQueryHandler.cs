using AutoMapper;
using MediatR;
using Edu_Block_dev.CQRS.Query.EduUser;
using Edu_Block_dev.Modal.DTO;
using Edu_Block.DAL.EF;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetRoleAndPermissionQueryHandler : IRequestHandler<GetRolesAndPermissionQuery, RolesAndPermissionDTO>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMapper _mapper;

        public GetRoleAndPermissionQueryHandler( IMapper mapper, EduBlockDataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<RolesAndPermissionDTO> Handle(GetRolesAndPermissionQuery request, CancellationToken cancellationToken)
        {
            RolesAndPermissionDTO RolesAndPermissionDTO = _context.Roles
                .Where( r => r.Id == request.UserRole.UserRoleGuid )
                                .GroupJoin(
                                    _context.RolePermissionMappings,
                                    r => r.Id,
                                    rpm => rpm.RoleId,
                                    (role, mappings) => new
                                    {
                                        Role = role,
                                        RolePermissionMappings = mappings.ToList() // Convert to list to avoid multiple enumeration
                                    })
                                .Select(rr => new RolesAndPermissionDTO
                                {
                                    Role = rr.Role,
                                    Permissions = rr.RolePermissionMappings.Select(rpm => rpm.PermissionDetail).ToList(),
                                    RolePermissionMappings = rr.RolePermissionMappings
                                })
                                .FirstOrDefault(); // Assuming you only expect one role based on the UserRoleGuid

            return RolesAndPermissionDTO;
        }
    }
}
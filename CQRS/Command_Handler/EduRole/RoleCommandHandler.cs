using System.Net;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduRole;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduRole
{
    public class RoleCommandHandler : IRequestHandler<AddRoleCommand, ApiResponse<object>>
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<PermissionDetail> _permissionDetailRepository;
        private readonly IRepository<RolePermissionMapping> _rolePermissionMappingRepository;
        private readonly IMapper _mapper;

        public RoleCommandHandler(IRepository<RolePermissionMapping> rolePermissionMappingRepository, IRepository<Role> roleRepository, IRepository<PermissionDetail> permissionDetailRepository, IMapper mapper)
        {
            _permissionDetailRepository = permissionDetailRepository;
            _rolePermissionMappingRepository = rolePermissionMappingRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }
        
        public async Task<ApiResponse<object>> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Role existingRole  =  await _roleRepository.FindAsync(r => r.NormalizedName == request.RoleRequestDTO.Name.ToUpper()) ;
                if(existingRole != null)
                {
                    return new ApiResponse<object>(HttpStatusCode.InternalServerError, error: "Role already exists", message: "Role already exists.");
                }
                Role role = _mapper.Map<Role>(request.RoleRequestDTO);
                await _roleRepository.AddAsync(role);
                if (request.RoleRequestDTO.PermissionList != null && request.RoleRequestDTO.PermissionList.Count() > 0)
                {
                    foreach (Guid permissionId in request.RoleRequestDTO.PermissionList)
                    {
                        PermissionDetail permission = await _permissionDetailRepository.FindAsync(p => p.Id == permissionId);
                        if (permission != null)
                        {
                            RolePermissionMapping rolePermissionMapping = new RolePermissionMapping()
                            {
                                 Id = Guid.NewGuid(),
                                 PermissionId = permission.Id,
                                 RoleId = role.Id
                            };
                            await _rolePermissionMappingRepository.AddAsync(rolePermissionMapping);
                        }
                    }
                }
                return new ApiResponse<object>(HttpStatusCode.OK, message: "Role added successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Unable to add role.");
            }
        }
    }
}
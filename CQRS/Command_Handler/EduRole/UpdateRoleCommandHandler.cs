using System.Net;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduRole;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduRole
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResponse<object>>
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;
        private readonly EduBlockDataContext _context;
        private readonly IRepository<PermissionDetail> _permissionDetailRepository;
        private readonly IRepository<RolePermissionMapping> _rolePermissionMappingRepository;


        public UpdateRoleCommandHandler(EduBlockDataContext context, IRepository<RolePermissionMapping> rolePermissionMappingRepository, IRepository<PermissionDetail> permissionDetailRepository,IRepository<Role> roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _permissionDetailRepository = permissionDetailRepository;
            _rolePermissionMappingRepository = rolePermissionMappingRepository;
            _mapper = mapper;
            _context = context;
        }
        
        public async Task<ApiResponse<object>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Role role = _mapper.Map<Role>(request.UpdateRoleRequestDTO);

                await _roleRepository.UpdateAsync(request.UpdateRoleRequestDTO.Guid ,role);

                var productsToRemove = _context.RolePermissionMappings.Where(p => p.RoleId == request.UpdateRoleRequestDTO.Guid).ToList<RolePermissionMapping>();
                _context.RemoveRange(productsToRemove);
                await _context.SaveChangesAsync();

                if (request.UpdateRoleRequestDTO.PermissionList != null && request.UpdateRoleRequestDTO.PermissionList.Count() > 0)
                {
                    foreach (Guid permissionId in request.UpdateRoleRequestDTO.PermissionList)
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
                return new ApiResponse<object>(HttpStatusCode.OK, message: "Role update successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Unable to update role.");
            }
        }
    }
}
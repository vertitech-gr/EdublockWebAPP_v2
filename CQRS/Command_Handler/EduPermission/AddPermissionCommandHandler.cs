using AutoMapper;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduPermission;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduPermission
{
	public class AddPermissionCommandHandler : IRequestHandler<AddPermissionCommand, ApiResponse<object>>
    {
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AddPermissionCommandHandler(IRepository<Permission> permissionRepository, IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<object>> Handle(AddPermissionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Permission permission = _mapper.Map<Permission>(request.PermissionRequestDTO);
                await _permissionRepository.AddAsync(permission);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: permission, message: "added permission");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, error: "added permission", message: "added permission");
            }
        }
    }
}


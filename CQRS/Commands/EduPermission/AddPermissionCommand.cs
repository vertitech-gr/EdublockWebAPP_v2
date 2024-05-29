using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduPermission
{
    public class AddPermissionCommand : IRequest<ApiResponse<object>>
    {
        public PermissionRequestDTO PermissionRequestDTO;
        public AddPermissionCommand(PermissionRequestDTO permissionRequestDTO)
        {
            PermissionRequestDTO = permissionRequestDTO;
        }
    }
}
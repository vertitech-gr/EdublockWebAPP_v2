using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduRole
{
    public class AddRoleCommand : IRequest<ApiResponse<object>>
    {
        public RoleRequestDTO RoleRequestDTO { get; set; }
        public AddRoleCommand(RoleRequestDTO roleRequestDTO)
        {
            RoleRequestDTO = roleRequestDTO;
        }
    }

    public class UpdateRoleCommand : IRequest<ApiResponse<object>>
    {
        public UpdateRoleRequestDTO UpdateRoleRequestDTO { get; set; }
        public UpdateRoleCommand(UpdateRoleRequestDTO updateRoleRequestDTO)
        {
            UpdateRoleRequestDTO = updateRoleRequestDTO;
        }
    }

    public class DeleteRoleCommand : IRequest<ApiResponse<object>>
    {
        public Guid Guid { get; set; }
        public DeleteRoleCommand(Guid guid)
        {
            Guid = guid;
        }
    }

}
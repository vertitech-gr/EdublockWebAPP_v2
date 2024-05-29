using Edu_Block.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduUserRequest
{
    public class CreateUserRequestCommand : IRequest<ApiResponse<object>>
    {
        public UserRequestDto UserRequestDto;
        public CommanUser? User;
        public CreateUserRequestCommand(CommanUser? user, UserRequestDto userRequestDto)
        {
            UserRequestDto = userRequestDto;
            User = user;
        }
    }
}
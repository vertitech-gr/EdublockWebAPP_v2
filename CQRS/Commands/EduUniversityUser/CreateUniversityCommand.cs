using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduUniversity
{
    public class CreateUniversityUserCommand : IRequest<ApiResponse<object>>
    {
        public UniversityUserRequestDTO UniversityUserRequestDTO;
        public CommanUser User;

        public CreateUniversityUserCommand(UniversityUserRequestDTO universityUserRequestDTO, CommanUser user)
        {
            UniversityUserRequestDTO = universityUserRequestDTO;
            User = user;
        }
    }

    public class UpdateLoginStatusCommand : IRequest<ApiResponse<object>>
    {
        public LoginStatusDTO LoginStatusDTO;
        public CommanUser User;

        public UpdateLoginStatusCommand(LoginStatusDTO loginStatusDTO, CommanUser user)
        {
            LoginStatusDTO = loginStatusDTO;
            User = user;
        }
    }
}
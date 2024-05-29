using Edu_Block_dev.Authorization;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduUser
{
    public class UserLoginCommand : IRequest<ApiResponse<object>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterUserCommand : IRequest<ApiResponse<object>>
    {
        public UserDTO UserDto { get; set; }
    }

    public class CreateStudentCommand : IRequest<ApiResponse<object>>
    {
        public UploadStudentDTO UploadStudentDTO;
        public CreateStudentCommand(UploadStudentDTO uploadStudentDTO)
        {
            UploadStudentDTO = uploadStudentDTO;
        }
    }

    public class VerifyEmailCommand : IRequest<ApiResponse<object>>
    {
        public VerifyEmailDTO VerifyEmailDto { get; set; }
    }

    public class ForgetPasswordCommand
    {
        public string Email { get; }

        public ForgetPasswordCommand(string email)
        {
            Email = email;
        }
    }

    public class ChangePasswordCommand
    {
        public ChangePasswordDTO ChangePasswordDto { get; }

        public ChangePasswordCommand(ChangePasswordDTO changePasswordDto)
        {
            ChangePasswordDto = changePasswordDto ?? throw new ArgumentNullException(nameof(changePasswordDto));
        }
    }

    public class ResendOtpCommand: IRequest<ApiResponse<object>>
    {
        public string Email { get; set; }
        public Role Type { get; set; }

        public ResendOtpCommand(string email, Role type)
        {
            Email = email;
            Type = type;
        }
    }
}
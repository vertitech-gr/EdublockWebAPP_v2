using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduUser
{
    public class RegisterAdminCommand : IRequest<ApiResponse<object>>
    {
        public UserDTO UserDto;
        public RegisterAdminCommand(UserDTO userDto)
        {
            UserDto = userDto;
        }
    }

    public class VerifyAdminEmailCommand : IRequest<ApiResponse<object>>
    {
        public VerifyEmailDTO VerifyEmailDto { get; set; }
        public VerifyAdminEmailCommand(VerifyEmailDTO verifyEmailDto)
        {
            VerifyEmailDto = verifyEmailDto;
        }
    }

    public class AdminLoginCommand : IRequest<ApiResponse<object>>
    {
        public LoginDTO _loginDTO;
        public AdminLoginCommand(LoginDTO loginDTO)
        {
            _loginDTO = loginDTO;
        }
    }

    public class ChangeAdminPasswordCommand : IRequest<ApiResponse<object>>
    {
        public ChangePasswordDTO ChangePasswordDto;
        public ChangeAdminPasswordCommand(ChangePasswordDTO changePasswordDto)
        {
            ChangePasswordDto = changePasswordDto;
        }
    }

    public class ResendAdminOtpCommand : IRequest<ApiResponse<object>>
    {
        public string Email;
        public ResendAdminOtpCommand(string email)
        {
            Email = email;
        }
    }

    //public class ForgetPasswordCommand
    //{
    //    public string Email { get; }

    //    public ForgetPasswordCommand(string email)
    //    {
    //        Email = email;
    //    }
    //}

}

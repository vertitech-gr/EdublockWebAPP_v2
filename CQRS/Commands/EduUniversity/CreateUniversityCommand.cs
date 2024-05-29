using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduUniversity
{
    public class CreateUniversityCommand : IRequest<ApiResponse<object>>
    {
        public UniversityRequestDTO UniversityRequestDTO;
        public CommanUser User { get; set; }

        public CreateUniversityCommand(UniversityRequestDTO UniversityRequestDTO, CommanUser user)
        {
            this.UniversityRequestDTO = UniversityRequestDTO;
            User = user;
        }
    }

    public class ResendUniversityCommand : IRequest<ApiResponse<object>>
    {
        public Guid Guid;
        public CommanUser User { get; set; }

        public ResendUniversityCommand(Guid guid, CommanUser user)
        {
            Guid = guid;
            User = user;
        }
    }

    public class ForgetUniversityPasswordCommand : IRequest<ApiResponse<object>>
    {
        public EmailRoleGuidDTO EmailRoleGuidDTO;
        public ForgetUniversityPasswordCommand(EmailRoleGuidDTO emailRoleGuidDTO)
        {
            EmailRoleGuidDTO = emailRoleGuidDTO;
        }
    }

    public class RegisterUniversityCommand : IRequest<ApiResponse<object>>
    {
        public UniversityRequestDTO UniversityRequestDTO;
        public RegisterUniversityCommand(UniversityRequestDTO UniversityRequestDTO)
        {
            this.UniversityRequestDTO = UniversityRequestDTO;
        }
    }

    public class UniversityDetailsCommand : IRequest<ApiResponse<object>>
    {
        public UniversityRequestDTO UniversityRequestDTO;
        public Guid UniversityId;
        public UniversityDetailsCommand(UniversityRequestDTO universityRequestDTO, Guid universityId)
        {
            this.UniversityRequestDTO = universityRequestDTO;
            UniversityId = universityId;
        }
    }

    public class UpdateUniversityDetailsCommand : IRequest<ApiResponse<object>>
    {
        public UniversityRequestDTO UniversityRequestDTO;
        public Guid UniversityId;
        public UpdateUniversityDetailsCommand(UniversityRequestDTO universityRequestDTO, Guid universityId)
        {
            this.UniversityRequestDTO = universityRequestDTO;
            UniversityId = universityId;
        }
    }

    public class VerifyUniversityEmailCommand : IRequest<ApiResponse<object>>
    {
        public VerifyEmailGuidDTO VerifyEmailDto;
        public VerifyUniversityEmailCommand(VerifyEmailGuidDTO verifyEmailDto)
        {
            this.VerifyEmailDto = verifyEmailDto;
        }
    }

    public class UniversityLoginCommand : IRequest<ApiResponse<object>>
    {
        public LoginGuidDTO UniversityLoginDTO;
        public UniversityLoginCommand(LoginGuidDTO universityLoginDTO)
        {
            UniversityLoginDTO = universityLoginDTO;
        }
    }

    public class ChangeUniversityPasswordCommand : IRequest<ApiResponse<object>>
    {
        public CommanUser CommanUser;
        public string NewPassword;
        public Guid RoleId;
        public ChangeUniversityPasswordCommand(CommanUser commanUser, string newPassword, Guid roleId)
        {
            RoleId = roleId;
            CommanUser = commanUser;
            NewPassword = newPassword;
        }
    }

    public class UpdateUniversityCommand : IRequest<ApiResponse<object>>
    {
        public CommanUser Admin;
        public Guid Guid;
        public UniversityUpdateRequestDTO UniversityUpdateRequestDTO;

        public UpdateUniversityCommand(CommanUser admin, Guid guid, UniversityUpdateRequestDTO universityUpdateRequestDTO)
        {
            Admin = admin;
            Guid = guid;
            UniversityUpdateRequestDTO = universityUpdateRequestDTO;
        }
    }

    public class ResendUniversityOtpCommand : IRequest<ApiResponse<object>>
    {
        public string Email;
        public Guid RoleId;
        public ResendUniversityOtpCommand(string email, Guid roleId)
        {
            Email = email;
            RoleId = roleId;
        }
    }

}

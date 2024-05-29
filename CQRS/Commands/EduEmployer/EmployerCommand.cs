using Edu_Block.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduEmployer
{
    public class EmployerCommand : IRequest<EmployerDTO>
    {
        public EmployerDTO employerDTO;
        public EmployerCommand(EmployerDTO employer)
        {
            employerDTO = employer;
        }
    }

    public class RegisterEmployeeCommand : IRequest<ApiResponse<object>>
    {
        public EmployerDTO employerDTO;
        public RegisterEmployeeCommand(EmployerDTO _employerDTO)
        {
            employerDTO = _employerDTO;
        }
    }

    public class CreateEmployeeTokenCommand : IRequest<ApiResponse<object>>
    {
        public CommanUser User;
        public string Name;
        public CreateEmployeeTokenCommand(string name, CommanUser user)
        {
            User = user;
            Name = name;
        }
    }

    public class DeleteEmployeeTokenCommand : IRequest<ApiResponse<object>>
    {
        public CommanUser User;
        public Guid EmpTokenID;
        public DeleteEmployeeTokenCommand(Guid empTokenID, CommanUser user)
        {
            User = user;
            EmpTokenID = empTokenID;
        }
    }

    public class EditEmployerCommand : IRequest<ApiResponse<object>>
    {
        public EditEmployerDTO EditEmployerDTO;

        public EditEmployerCommand(EditEmployerDTO editEmployerDTO)
        {
            EditEmployerDTO = editEmployerDTO;
        }
    }

    public class EmployeeLoginCommand : IRequest<ApiResponse<object>>
    {
        public EmployeeLoginDTO _employeeLoginDTO;
        public EmployeeLoginCommand(EmployeeLoginDTO employeeLoginDTO)
        {
            _employeeLoginDTO = employeeLoginDTO;
        }
    }

    public class EmployeeChangePasswordCommand : IRequest<ApiResponse<object>>
    {
        public ChangePasswordDTO ChangePasswordDTO;
        public EmployeeChangePasswordCommand(ChangePasswordDTO changePasswordDTO)
        {
            ChangePasswordDTO = changePasswordDTO;
        }
    }

    public class ForgetEmployeePasswordCommand : IRequest<ApiResponse<object>>
    {
        public string Email;
        public ForgetEmployeePasswordCommand(string email)
        {
            Email = email;
        }
    }

    public class VerifyEmployeeEmailCommand : IRequest<ApiResponse<object>>
    {
        public VerifyEmailDTO VerifyEmailDto { get; set; }
    }

}

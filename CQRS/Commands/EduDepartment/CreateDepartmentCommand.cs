using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduDepartment
{
    public class CreateDepartmentCommand : IRequest<ApiResponse<object>>
    {
        public DepartmentDTO DepartmentDto { get; }
        public CommanUser User { get; }
        public CreateDepartmentCommand(CommanUser user, DepartmentDTO hodDto)
        {
            DepartmentDto = hodDto;
            User = user;
        }
    }

    public class UploadStudentCommand : IRequest<ApiResponse<object>>
    {
        public UploadStudentRequestDTO UploadStudentRequestDTO;
        public UploadStudentCommand(UploadStudentRequestDTO uploadStudentRequestDTO)
        {
            UploadStudentRequestDTO = uploadStudentRequestDTO;
        }
    }

    public class UpdateDepartmentCommand : IRequest<ApiResponse<object>>
    {
        public CommanUser User { get; }
        public DepartmentRequestDTO DepartmentRequestDTO { get; }
        public UpdateDepartmentCommand(CommanUser user, DepartmentRequestDTO departmentRequestDTO)
        {
            User = user;
            DepartmentRequestDTO = departmentRequestDTO;
        }
    }

    public class DeleteDepartmentCommand : IRequest<ApiResponse<object>>
    {
        public CommanUser User { get; }
        public Guid Guid;
        public DeleteDepartmentCommand(CommanUser user, Guid guid)
        {
            User = user;
            Guid = guid;
        }
    }
}
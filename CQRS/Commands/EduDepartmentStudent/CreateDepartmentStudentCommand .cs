using Edu_Block.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduCertificate
{
    public class CreateDepartmentStudentCommand : IRequest<ApiResponse<object>>
    {
        public DepartmentStudent DepartmentStudent;
        public CreateDepartmentStudentCommand(DepartmentStudent departmentStudent)
        {
            DepartmentStudent = departmentStudent;
        }
    }
}
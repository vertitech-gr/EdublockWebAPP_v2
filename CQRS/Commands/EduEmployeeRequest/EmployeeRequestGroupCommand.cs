using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EmployeeRequest
{
    public class EmployeeRequestGroupCommand : IRequest<EmployeeRequestGroupDTO>
    {
        public EmployeeRequestGroupDTO RequestGroupDTO { get; }
        public EmployeeRequestGroupCommand(EmployeeRequestGroupDTO requestGroupDTO)
        {
            RequestGroupDTO = requestGroupDTO;
        }
    }
}

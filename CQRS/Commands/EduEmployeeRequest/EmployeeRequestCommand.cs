using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EmployeeRequest
{
    public class EmployeeRequestCommand : IRequest<EmployeeRequestResponse>
    {
        public EmployeeRequestDTO RequestDto { get; }
        public CommanUser Employer { get; set; }
        public EmployeeRequestCommand(EmployeeRequestDTO requestDto, CommanUser employer)
        {
            RequestDto = requestDto;
            Employer = employer;
        }
    }
}

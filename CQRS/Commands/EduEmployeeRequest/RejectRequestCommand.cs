using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EmployeeRequest
{
    public class RejectRequestCommand : IRequest<EmployeeRequestDTO>
    {
        public Guid id { get; }
        public Guid UserId { get; }
        public RejectRequestCommand(Guid _id, Guid _user_id)
        {
            UserId = _user_id;
            id = _id;

        }
    }

}

using Edu_Block_dev.Modal.Base;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduUserRequest
{
    public class UpdateRequestStatusCommand : IRequest<ApiResponse<object>>
    {
        public Guid Guid;
        public MessageStatus Status;
        public UpdateRequestStatusCommand(Guid guid, MessageStatus status)
        {
            Guid = guid;
            Status = status;
        }
    }
}

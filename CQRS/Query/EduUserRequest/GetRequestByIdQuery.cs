using MediatR;
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.CQRS.Query.EduUserRequest
{
    public class GetRequestByDetailsQuery : IRequest<Request>
    {
        public Guid SenderId { get; }
        public Guid ReceiverId { get; }

        public GetRequestByDetailsQuery(Guid senderId, Guid receiverId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
        }
    }
}

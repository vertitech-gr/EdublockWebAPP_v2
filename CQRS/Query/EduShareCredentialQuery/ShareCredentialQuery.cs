using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduShareCredentialQuery
{
    public class ShareCredentialQuery : IRequest<ShareCredentialDTO>
    {
        public Guid Id { get; set; }
    }
}

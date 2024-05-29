using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduCertificate
{
    public class ShareCommand : IRequest<ShareResponse>
    {
        public Guid CertificateId { get; set; }
        public string Email { get; set; }

    }
    public class ShareResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}

using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduCertificate
{
    public class InsertCertificateCommand : IRequest<ApiResponse<object>>
    {
        public Certificate certificate;
        public InsertCertificateCommand(Certificate certificate)
        {
            this.certificate = certificate;
        }
    }
}
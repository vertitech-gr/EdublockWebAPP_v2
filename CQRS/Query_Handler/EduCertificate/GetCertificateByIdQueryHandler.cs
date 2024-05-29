using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduCertificate
{
    public class GetCertificateByIdQueryHandler : IRequestHandler<GetCertificatesQueryById, Certificate>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<Certificate> _certificate;

        public GetCertificateByIdQueryHandler(EduBlockDataContext context, IRepository<Certificate> certificate)
        {
            _context = context;
            _certificate = certificate;
        }

        public async Task<Certificate> Handle(GetCertificatesQueryById request, CancellationToken cancellationToken)
        {
            return await _certificate.FindAsync(u => u.Id.Equals(request.CertificateId));
        }
    }
}

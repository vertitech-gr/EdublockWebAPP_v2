using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduCertificate
{
    public class CertificateExistsQueryHandler : IRequestHandler<CertificateExistsQuery, bool>
    {
        private readonly EduBlockDataContext _context;

        public CertificateExistsQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CertificateExistsQuery request, CancellationToken cancellationToken)
        {
            bool certificateExists = await _context.Certificates
             .AnyAsync(c => c.Id == request.CertificateId && !c.IsDeleted, cancellationToken);

            return certificateExists;
        }
    }
}

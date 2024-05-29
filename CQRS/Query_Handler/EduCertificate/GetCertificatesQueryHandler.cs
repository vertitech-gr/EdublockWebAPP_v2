using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduCertificate
{
    public class GetCertificatesQueryHandler : IRequestHandler<GetCertificatesQuery, List<Certificate>>
    {
        private readonly EduBlockDataContext _context;

        public GetCertificatesQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<List<Certificate>> Handle(GetCertificatesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Certificate> certificates = null;
            if (request.User != null)
            {
              certificates = await _context.Certificates
             .Where(u => u.UserProfileId.Equals(request.User.UserProfile.Id))
             .ToListAsync();
            }
            else
            {
              certificates = await _context.Certificates
             .ToListAsync();
            }

            return certificates.ToList();
        }
    }
}

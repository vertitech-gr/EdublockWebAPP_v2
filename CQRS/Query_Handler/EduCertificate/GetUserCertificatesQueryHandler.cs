using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduUser;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduCertificate
{
    public class GetUserCertificatesQueryHandler : IRequestHandler<GetUserCertificatesQuery, List<Certificate>>
    {
        private readonly EduBlockDataContext _context;

        public GetUserCertificatesQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<List<Certificate>> Handle(GetUserCertificatesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Certificate> certificates = null;
            if (request.UserProfileID != null)
            {
              certificates = await _context.Certificates
             .Where(u => u.UserProfileId.Equals(request.UserProfileID))
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

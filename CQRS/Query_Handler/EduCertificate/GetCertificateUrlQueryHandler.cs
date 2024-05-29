using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduCertificate
{
    public class GetCertificateUrlQueryHandler : IRequestHandler<GetCertificateUrlQuery, string>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<Certificate> _certificate;
        private readonly IConfiguration _configuration;


        public GetCertificateUrlQueryHandler(EduBlockDataContext context, IConfiguration configuration, IRepository<Certificate> certificate)
        {
            _context = context;
            _certificate = certificate;
            _configuration = configuration;
        }

        public async Task<string> Handle(GetCertificateUrlQuery request, CancellationToken cancellationToken)
        {
            Certificate certificate = await _certificate.FindAsync(u => u.Id.Equals(request.CertificateId));
            var baseUrl = _configuration.GetSection("Base:url").Value;
            return baseUrl + "/" + certificate.Path + "/" + certificate.FileName;
        }
    }
}

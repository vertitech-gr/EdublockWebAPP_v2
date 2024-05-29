using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduEnvelope
{
    public class ShareEnvelopeQueryByIdHandler : IRequestHandler<SahredEnvelopQueryById, EnvelopResponseDTO>
    {
        private readonly IRepository<Envelope> _envelope;
        private readonly IRepository<Share> _share;
        private readonly IMediator _mediator;
        private readonly EduBlockDataContext _context;

        public ShareEnvelopeQueryByIdHandler(EduBlockDataContext context, IRepository<Envelope> envelope, IRepository<Share> share, IMediator mediator)
        {
            _context = context;
            _envelope = envelope;
            _mediator = mediator;
            _share = share;
        }

        public async Task<EnvelopResponseDTO> Handle(SahredEnvelopQueryById request, CancellationToken cancellationToken)
        {

            var shareRes = _context.Shares.Where(s => s.Token == request.Token);


            Share share = await _share.FindAsync((e) => e.Token == request.Token);

          
            var shareDetailResult = shareRes.GroupJoin(
                _context.ShareCredentials,
                share => share.Id,
                shareCredential => shareCredential.ShareId,
                (share, shareCredentials) => new
                {
                    Share = share,
                    CredentialIds = shareCredentials.Select(sc => sc.CredentialId).ToList()
                })
                .SelectMany(
                    x => x.CredentialIds.DefaultIfEmpty(),
                    (x, credentialId) => new
                    {
                        x.Share,
                        CredentialId = credentialId,
                        Credential = _context.Certificates.FirstOrDefault(c => c.Id == credentialId)
                    })
                .GroupBy(
                    x => x.Share,
                    (key, group) => new
                    {
                        Share = key,
                        Credentials = group.Where(g => g.Credential != null).Select(g => g.Credential).ToList()
                    })
                .ToList();
            
            //ShareCertificateCredentialsDTO shareCredentialCertificates = _context.ShareCredentials
            //  .GroupJoin(_context.Certificates, sh => sh.CredentialId, c => c.Id, (sh, c) => new
            //  {
            //      ShareCredential = sh,
            //      Certificates = c.ToList()
            //  })
            //  .Where(sh => sh.ShareCredential.ShareId == share.Id)
            //  .Select(d => new ShareCertificateCredentialsDTO
            //  {
            //      ShareCredential = d.ShareCredential,
            //      Certificates = d.Certificates
            //  }).FirstOrDefault();

            Envelope env = await _envelope.FindAsync(u => u.Id.Equals(share.ResourceId));

            UserProfile userProfile = _context.UserProfiles.Where(up => up.Id == env.UserProfileId).FirstOrDefault(); 

            List<CertificateView> certificateDetails = new List<CertificateView>();

            foreach (Certificate certificate in shareDetailResult[0].Credentials)
            {
                if (certificate != null)
                {
                    CertificateView certificateDetail = new CertificateView()
                    {
                        Id = certificate.Id,
                        Name = certificate.DegreeName,
                        status = certificate.Status,
                        degreeType = certificate.DegreeType,
                        degreeName = certificate.DegreeName,
                        fileName = certificate.FileName,
                        path = certificate.Path,
                        degreeAwardedDate = certificate.DegreeAwardedDate,
                        dateOfBirth = certificate.DateOfBirth,
                        issuanceDate = certificate.IssuanceDate,
                        expireDate = certificate.ExpireDate,
                        certificateTemplateID = certificate.CertificateTemplateId,
                        userProfileID = certificate.UserProfileId,
                        credentialsJson = certificate.CredentialsJson
                    };
                    certificateDetails.Add(certificateDetail);
                }
            }

            return new EnvelopResponseDTO
            {
                Name = userProfile.Name,
                Id = env.Id,
                Status = RequestStatus.Pending,
                Credentials = certificateDetails,
                IsDeletedAt = env.IsDeletedAt,
                CreatedAt = env.CreatedAt,
                UpdatedAt = env.UpdatedAt,
                CreatedBy = env.CreatedBy,
                UpdatedBy = env.UpdatedBy
            };
        }
    }
}

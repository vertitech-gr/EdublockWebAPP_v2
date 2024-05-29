using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduEnvelope
{
    public class EnvelopeRequestGroupQueryHandler : IRequestHandler<EnvelopeRequestGroupQuery, List<CertificateView>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMediator _mediator;
        public EnvelopeRequestGroupQueryHandler(EduBlockDataContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<List<CertificateView>> Handle(EnvelopeRequestGroupQuery request, CancellationToken cancellationToken)
        {

            List<EnvelopGroup> envelopGroups = await _context.EnvelopGroups
                .Where(e => e.EnvelopeID == request.EnvelopeId)
                .ToListAsync(cancellationToken);

            List<EnvelopeRequestGroupDTO> envelopeRequestGroups = new List<EnvelopeRequestGroupDTO>();

            List<EnvelopResponseDTO> responseList = new List<EnvelopResponseDTO>();
            List<CertificateView> certificateDetails = new List<CertificateView>();

            List<Guid> CredentialList = await _context.EnvelopGroups
                    .Where(r => r.EnvelopeID == request.EnvelopeId)
                    .Select(r => r.CertificateId)
                    .ToListAsync(cancellationToken);

            foreach (Guid certificateId in CredentialList)
            {
                var certificate = await _mediator.Send(new GetCertificatesQueryById(certificateId));
                if(certificate != null)
                {
                    CertificateView certificateDetail = new CertificateView()
                    {
                        Id = certificate.Id,
                        Name = certificate.DegreeName,
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
                        credentialsJson = certificate.CredentialsJson,
                        status = certificate.Status
                    };
                    certificateDetails.Add(certificateDetail);
                }
            }

            return certificateDetails;
        }
    }
}

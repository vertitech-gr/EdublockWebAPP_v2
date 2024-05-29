using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduEnvelope
{
    public class EnvelopeRequestGroupQuery : IRequest<List<CertificateView>>
    {
        public Guid EnvelopeId { get; set; }

        public EnvelopeRequestGroupQuery(Guid envelopeId)
        {
            EnvelopeId = envelopeId;
        }
    }
}

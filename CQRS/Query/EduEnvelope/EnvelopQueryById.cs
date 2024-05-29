using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduEnvelope
{
    public class SahredEnvelopQueryById : IRequest<EnvelopResponseDTO>
    {
        public string Token;

        public SahredEnvelopQueryById(string token)
        {
            Token = token;
        }
    }

    public class EnvelopQueryById : IRequest<EnvelopResponseDTO>
    {
        public Guid EnvelopeId { get; set; }

        public EnvelopQueryById(Guid envelopeId)
        {
            EnvelopeId = envelopeId;
        }
    }
}

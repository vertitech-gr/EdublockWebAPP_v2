using Edu_Block_dev.CQRS.Commands;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduEnvelope
{
    public class ShareEnvelopeByIdQuery : IRequest<EnvelopResponseDTO>
    {
        public Guid EnvelopeId { get; set; }
        public List<Guid> Credential { get; set; }
        public string Email { get; set; }

        public Guid Userid { get; }

        public ShareEnvelopeByIdQuery(Guid userid, Guid envelopeId, List<Guid> credential, string email)
        {
            EnvelopeId = envelopeId;
            Credential = credential;
            Email = email;
            Userid = userid;
        }
    }

    public class ShareEnvelopeUrlQuery : IRequest<string>
    {
        public Guid EnvelopeId { get; set; }

        public ShareEnvelopeUrlQuery(Guid envelopeId)
        {
            EnvelopeId = envelopeId;
        }
    }


}

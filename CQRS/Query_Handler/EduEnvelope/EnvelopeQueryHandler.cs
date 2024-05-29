using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduEnvelope
{
    public class EnvelopeQueryHandler : IRequestHandler<EnvelopeQuery, Envelope>
    {
        private readonly IRepository<Envelope> _envelope;
        public EnvelopeQueryHandler(EduBlockDataContext context, IRepository<Envelope> envelope)
        {
            _envelope = envelope;
        }
        public async Task<Envelope> Handle(EnvelopeQuery request, CancellationToken cancellationToken)
        {
            var _envelopes = await _envelope.FindAsync(u => u.Name.Equals(request._name));

            return _envelopes;
        }
    }
}
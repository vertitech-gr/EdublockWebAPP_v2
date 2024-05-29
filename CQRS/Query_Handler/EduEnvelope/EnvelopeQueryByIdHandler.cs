using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduEnvelope
{
    public class EnvelopeQueryByIdHandler : IRequestHandler<EnvelopQueryById, EnvelopResponseDTO>
    {
        private readonly IRepository<Envelope> _envelope;
        private readonly IMediator _mediator;
        public EnvelopeQueryByIdHandler(EduBlockDataContext context, IRepository<Envelope> envelope, IMediator mediator)
        {
            _envelope = envelope;
            _mediator = mediator;
        }
        public async Task<EnvelopResponseDTO> Handle(EnvelopQueryById request, CancellationToken cancellationToken)
        {
            Envelope env = await _envelope.FindAsync(u => u.Id.Equals(request.EnvelopeId));

            if (env == null)
            {
                return null;
            }

            List<CertificateView> certificateDetails = await _mediator.Send(new EnvelopeRequestGroupQuery(env.Id));

            return new EnvelopResponseDTO
            {
                Name = env.Name,
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

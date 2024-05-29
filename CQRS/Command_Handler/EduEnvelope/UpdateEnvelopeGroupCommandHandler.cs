using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduEnvelope;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduEnvelope
{
    public class UpdateEnvelopeGroupCommandHandler : IRequestHandler<UpdateEnvelopeGroupCommand, EnvelopGroup>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public UpdateEnvelopeGroupCommandHandler(EduBlockDataContext context, IMapper mapper, IMediator mediator)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<EnvelopGroup> Handle(UpdateEnvelopeGroupCommand request, CancellationToken cancellationToken)
        {
            Guid envelopeId = request.envelopeGroupDTO.EnvelopeId;
            List<CertificateView> certificateViews = await _mediator.Send(new EnvelopeRequestGroupQuery(envelopeId));
            List<Guid> newCredentials = request.envelopeGroupDTO.Credentials;
            List<Guid> certificateToAdd = newCredentials.Except(certificateViews.Select(cert => cert.Id)).ToList();
            List<Guid> certificateToRemove = certificateViews.Where(cert => !newCredentials.Contains(cert.Id)).ToList().Select(s => s.Id).ToList<Guid>();

            foreach(Guid CertificateId in certificateToRemove)
            {
                EnvelopGroup envelopeGroup = new EnvelopGroup()
                {
                    Id = envelopeId,
                    CertificateId = CertificateId
                };
                await _mediator.Send(new RemoveEnvelopeGroupCommand(envelopeGroup));
            }

            foreach (Guid CertificateId in certificateToAdd)
            {
                EnvelopGroup envelopeGroup = new EnvelopGroup()
                {
                    CertificateId = CertificateId,
                    EnvelopeID = envelopeId,
                    Status = RequestStatus.Pending.ToString(),
                };
                await _mediator.Send(new AddEnvelopeGroupCommand(envelopeGroup));
            }

            return new EnvelopGroup();
        }
    }
}

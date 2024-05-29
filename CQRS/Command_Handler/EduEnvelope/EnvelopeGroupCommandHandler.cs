using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduEnvelope
{
    public class EnvelopeGroupCommandHandler : IRequestHandler<EnvelopeGroupCommand, EnvelopGroup>
    {
        private readonly EduBlockDataContext _dbContext;
        private readonly IMapper _mapper;

        public EnvelopeGroupCommandHandler(EduBlockDataContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<EnvelopGroup> Handle(EnvelopeGroupCommand request, CancellationToken cancellationToken)
        {
            if (request._envelopeRequestGroupDTO == null)
            {
                return null;
            }

            EnvelopGroup envelopGroupEntity = new EnvelopGroup()
            {
                CertificateId = request._envelopeRequestGroupDTO.CertificateId,
                EnvelopeID = request._envelopeRequestGroupDTO.EnvelopeId,
                Status = request._envelopeRequestGroupDTO.Status.ToString()

            };

            _dbContext.Add(envelopGroupEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return envelopGroupEntity;
        }

    }
}

using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Command_Handler.EduEnvelope
{
    public class RemoveEnvelopeGroupCommandHandler : IRequestHandler<RemoveEnvelopeGroupCommand, EnvelopGroup>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMapper _mapper;

        public RemoveEnvelopeGroupCommandHandler(EduBlockDataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<EnvelopGroup> Handle(RemoveEnvelopeGroupCommand request, CancellationToken cancellationToken)
        {
            // Map EnvelopeGroupDTO to EnvelopGroup using AutoMapper
            // var envelopeGroup = _mapper.Map<EnvelopGroup>(request.envelopeGroupDTO);

            // Find and remove EnvelopeGroup records based on EnvelopeId and CertificateId
            var envelopeGroupsToRemove = await _context.EnvelopGroups
                .Where(eg => eg.EnvelopeID == request.envelopeGroup.Id && eg.CertificateId == request.envelopeGroup.CertificateId)
                .ToListAsync();

            if (envelopeGroupsToRemove.Any())
            {
                _context.EnvelopGroups.RemoveRange(envelopeGroupsToRemove);
                await _context.SaveChangesAsync();
            }
            return request.envelopeGroup;
        }
    }

}

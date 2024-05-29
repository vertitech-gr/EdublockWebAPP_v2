using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduEnvelope
{
    public class AddEnvelopeGroupCommandHandler : IRequestHandler<AddEnvelopeGroupCommand, EnvelopGroup>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMapper _mapper;

        public AddEnvelopeGroupCommandHandler(EduBlockDataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<EnvelopGroup> Handle(AddEnvelopeGroupCommand request, CancellationToken cancellationToken)
        {
            _context.EnvelopGroups.Add(request.envelopeGroup);
            await _context.SaveChangesAsync();
            return request.envelopeGroup;
        }
    }
}

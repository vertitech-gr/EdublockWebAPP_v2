using Edu_Block.DAL.EF;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.Modal.Enum;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduEnvelope
{
    public class EnvelopeCommand : IRequest<ApiResponse<object>>
    {
        public EnvelopeDTOC envelopeDTOs { get; }
        public CommanUser User { get; }
        public EnvelopeShareType Type  { get; set; }

        public EnvelopeCommand(EnvelopeDTOC envelopeDTO, CommanUser user, EnvelopeShareType type)
        {
            envelopeDTOs = envelopeDTO;
            User = user;
            Type = type;
        }
    }

    public class EnvelopeGroupCommand : IRequest<EnvelopGroup>
    {
        public EnvelopeRequestGroupDTO _envelopeRequestGroupDTO { get; }
        public EnvelopeGroupCommand(EnvelopeRequestGroupDTO envelopeRequestGroupDTO)
        {
            _envelopeRequestGroupDTO = envelopeRequestGroupDTO;
        }
    }

    public class AddEnvelopeGroupCommand : IRequest<EnvelopGroup>
    {
        public EnvelopGroup envelopeGroup { get; }
        public AddEnvelopeGroupCommand(EnvelopGroup _envelopeGroup)
        {
            envelopeGroup = _envelopeGroup;
        }
    }

    public class RemoveEnvelopeGroupCommand : IRequest<EnvelopGroup>
    {
        public EnvelopGroup envelopeGroup { get; }
        public RemoveEnvelopeGroupCommand(EnvelopGroup _envelopeGroup)
        {
            envelopeGroup = _envelopeGroup;
        }
    }

    public class UpdateEnvelopeGroupCommand : IRequest<EnvelopGroup>
    {
        public EnvelopeGroupDTO envelopeGroupDTO { get; }
        public UpdateEnvelopeGroupCommand(EnvelopeGroupDTO _envelopeGroupDTO)
        {
            envelopeGroupDTO = _envelopeGroupDTO;
        }
    }
}
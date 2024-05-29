using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands
{
    public class CertificateDIDCommand : IRequest<DockIoDID>
    {
        public DockIoDID dockDID;
        public CertificateDIDCommand(DockIoDID dockDID)
        {
            this.dockDID = dockDID;
        }
    }

}

using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduCertificate
{
    public class CertrificateDIDCommandHandler : IRequestHandler<CertificateDIDCommand, DockIoDID>
    {
        private readonly EduBlockDataContext _context;

        public CertrificateDIDCommandHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<DockIoDID> Handle(CertificateDIDCommand request, CancellationToken cancellationToken)
        {
            _context.Add(request.dockDID);
            await _context.SaveChangesAsync();
            return request.dockDID;
        }
    }
}

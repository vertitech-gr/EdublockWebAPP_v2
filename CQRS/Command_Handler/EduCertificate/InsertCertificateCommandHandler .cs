using System.Net;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduCertificate;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduCertificate
{
    public class InsertCertificateCommandHandler : IRequestHandler<InsertCertificateCommand, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;

        public InsertCertificateCommandHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<object>> Handle(InsertCertificateCommand request, CancellationToken cancellationToken)
        {
            try {
                _context.Add(request.certificate);
                var result = await _context.SaveChangesAsync(cancellationToken);
                return new ApiResponse<object>(HttpStatusCode.OK, data: result, message: "Certificate created successfully");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(HttpStatusCode.OK, data: null, message: "Certificate created successfully");
            }
        }
    }
}
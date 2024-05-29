using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduCertificate;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduCertificate
{
    public class ShareCommandHandler : IRequestHandler<ShareCommand, ShareResponse>
    {
        private readonly IMediator _mediator;
        private readonly Util _util;
        private readonly IConfiguration _configuration;


        public ShareCommandHandler(IMediator mediator, Util util, IConfiguration configuration)
        {
            _mediator = mediator;
            _util = util;
            _configuration = configuration;
        }
        public async Task<ShareResponse> Handle(ShareCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var certificate = await _mediator.Send(new GetCertificatesQueryById(request.CertificateId));
                var baseUrl = _configuration.GetSection("Base:url").Value;

                if (certificate != null)
                {
                    string contentUrl = baseUrl + "/" + certificate.Path + "/" + certificate.FileName;
                    string subject = "Certificate Shared";
                    string content = $"Certificate details:" + contentUrl;
                    string recipientEmail = request.Email;
                    _util.SendSimpleMessage(subject, recipientEmail, content, "View Certificate", contentUrl);

                    return new ShareResponse { IsSuccess = true, Message = "Certificate shared successfully." };
                }
                return new ShareResponse { IsSuccess = true, Message = "Certificate not found." };
            }
            catch (Exception ex)
            {
                return new ShareResponse { IsSuccess = false, Message = "An error occurred while sharing the certificate." };
            }
        }
    }

}

using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using MediatR;
using Microsoft.Extensions.Hosting.Internal;
using NuGet.Packaging;

namespace Edu_Block_dev.CQRS.Query_Handler.EduEnvelope
{
    public class ShareEnvelopeUrlHandler : IRequestHandler<ShareEnvelopeUrlQuery, string>
    {
        private readonly IRepository<Envelope> _envelope;
        private readonly Util _util;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public ShareEnvelopeUrlHandler (IWebHostEnvironment hostingEnvironment,EduBlockDataContext context, IConfiguration configuration, Util util, IRepository<Envelope> envelope)
        {
            _envelope = envelope;
            _configuration = configuration;
            _util = util;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<string> Handle(ShareEnvelopeUrlQuery request, CancellationToken cancellationToken)
        {
            var envelope = await _envelope.FindAsync(u => u.Id.Equals(request.EnvelopeId));

            var baseUrl = _configuration.GetSection("Base:url").Value;

            return baseUrl + "/" + envelope.Path + ".zip";

        }
    }
}

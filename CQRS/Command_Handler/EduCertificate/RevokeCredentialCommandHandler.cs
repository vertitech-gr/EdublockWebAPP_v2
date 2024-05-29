using Edu_Block.DAL;
using MediatR;
using AutoMapper;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using System.Text;
using Newtonsoft.Json;
using Edu_Block_dev.Modal.DTO;
using Edu_Block.DAL.EF;
using static QRCoder.PayloadGenerator;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class RevokeCredentialCommandHandler : IRequestHandler<RevokeCredentialCommand, ApiResponse<object>>
    {
        private readonly IRepository<Certificate> _certificateRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly EduBlockDataContext _context;
        private readonly Util _util;


        public RevokeCredentialCommandHandler(IConfiguration configuration, Util util, EduBlockDataContext context, IMapper mapper, IRepository<Certificate> certificateRepository)
        {
            _certificateRepository = certificateRepository;
            _mapper = mapper;
            _configuration = configuration;
            _context = context;
            _util = util;
        } 

        public async Task<ApiResponse<object>> Handle(RevokeCredentialCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var client = new HttpClient();
                var apiKey = _configuration.GetSection("Dock:ApiKey").Value;
                Certificate existingCertificate = await _certificateRepository.FindAsync(u => u.Id == request.Guid );

                UserProfile userProfile = _context.UserProfiles.Where(up => up.Id == existingCertificate.UserProfileId).FirstOrDefault();
                User user = _context.User.Where(up => up.Id == userProfile.UserID ).FirstOrDefault();

                var revokeRequest = new RevokeCredentialRequest
                {
                    Action = existingCertificate.Status ? "unrevoke" : "revoke",
                    CredentialIds = new List<string>
                    {
                        existingCertificate.CertificateID
                    }
                };
                var jsonContent = JsonConvert.SerializeObject(revokeRequest);
                var response = await SendHttpRequest(client, jsonContent, existingCertificate.RegistryID, apiKey);
                RevokeRequestDTO RevokeRequestDTO = JsonConvert.DeserializeObject<RevokeRequestDTO>(response);
                var revokeChangeStatus = RevokeRequestDTO.Data.RevokeIds.FirstOrDefault();

                if (revokeChangeStatus == 1)
                {
                    existingCertificate.Status = !existingCertificate.Status;
                    await _certificateRepository.UpdateAsync(existingCertificate.Id, existingCertificate);

                    var subject = "Revoke certificate on EduBlock";
                    var content = $"<p>Dear { user.Name }, your {existingCertificate.DegreeName} certificate has been revoked.</p>" +
                    $"<br>";

                    _util.SendSimpleMessage(subject, user.Email, content, null, null);

                    return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: existingCertificate,  message: $"Certificate {revokeRequest.Action}d successfully"  );

                }
                else {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: "Certificate update Unsuccessfull");
                }



            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: "Certificate update Unsuccessfull");

            }
        }

        private async Task<string> SendHttpRequest(HttpClient client, string jsonPayload, string registryId, string apiKey)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://api-testnet.dock.io/registries/{registryId}");
            request.Headers.Add("dock-api-token", apiKey);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var result = await client.SendAsync(request);
            return await result.Content.ReadAsStringAsync();
        }
    }
}
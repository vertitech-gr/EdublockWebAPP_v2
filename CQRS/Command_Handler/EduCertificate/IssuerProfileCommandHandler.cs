using System.Text;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.Modal.Dock;
using MediatR;
using Newtonsoft.Json;

namespace Edu_Block_dev.CQRS.Command_Handler
{
    public class IssuerProfileCommandHandler : IRequestHandler<IssuerProfileCommand, CreateHandlerResponse>
    {
        private readonly IConfiguration _configuration;

        public IssuerProfileCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<CreateHandlerResponse> Handle(IssuerProfileCommand request, CancellationToken cancellationToken)
        {
            return await this.IssuerProfileApiCall(request.CreateHandlerResponse, request.issuerDTO);
        }
        public async Task<CreateHandlerResponse> IssuerProfileApiCall(CreateHandlerResponse createHandlerResponse, IssuerDTO issuerDTO)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-testnet.dock.io/profiles");
            var apiKey = _configuration.GetSection("Dock:ApiKey").Value;
            request.Headers.Add("dock-api-token", apiKey);
            var profileRequest = new ProfileRequestDTO
            {
                name = issuerDTO.Name,
                description = issuerDTO.Description,
                logo = issuerDTO.Logo,
                type = "dock",
                did = createHandlerResponse.Did
            };
            var jsonPayload = JsonConvert.SerializeObject(profileRequest);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                return createHandlerResponse;
            }
            else
            {
                return new CreateHandlerResponse();
            }
        }
    }
}
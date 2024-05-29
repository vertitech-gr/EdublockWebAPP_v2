using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.Modal.Dock;
using MediatR;
using Newtonsoft.Json;

namespace Edu_Block_dev.CQRS.Command_Handler
{
    public class CreateHolderCommandHandler : IRequestHandler<CreateDockHandlerCommand, CreateHandlerResponse>
    {
        private readonly IConfiguration _configuration;

        public CreateHolderCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public async Task<CreateHandlerResponse> Handle(CreateDockHandlerCommand request, CancellationToken cancellationToken)
        {
            CreateHandlerResponse createHandlerResponse = await this.CreateHolderApiCall();
            createHandlerResponse.Success = true;
            return createHandlerResponse;
        }
        public async Task<CreateHandlerResponse> CreateHolderApiCall()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-testnet.dock.io/dids");

            var apiKey = _configuration.GetSection("Dock:ApiKey").Value;
            request.Headers.Add("dock-api-token", apiKey);

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                CreateHandlerResponse createHandlerResponse = JsonConvert.DeserializeObject<CreateHandlerResponse>(response);
                return createHandlerResponse;
            }
            else
            {
                return new CreateHandlerResponse();
            }
        }
    }
}
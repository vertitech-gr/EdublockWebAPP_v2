using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.Modal.Dock;
using MediatR;
using Newtonsoft.Json;

namespace Edu_Block_dev.CQRS.Command_Handler
{
    public class CreateIssuerCommandHandler : IRequestHandler<CreateDockIssuerCommand, CreateIssuerResponse>
    {
        private readonly IMediator _mediator;
        public CreateIssuerCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<CreateIssuerResponse> Handle(CreateDockIssuerCommand request, CancellationToken cancellationToken)
        {
            CreateIssuerResponse createIssuerResponse = await this.CreateIssuerApiCall();
            //createIssuerResponse.Success = true;
            return createIssuerResponse;
        }
        public async Task<CreateIssuerResponse> CreateIssuerApiCall()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-testnet.dock.io/dids");
            request.Headers.Add("dock-api-token", "eyJzY29wZXMiOlsidGVzdCIsImFsbCJdLCJzdWIiOiIxNDM2MyIsInNlbGVjdGVkVGVhbUlkIjoiMTgzMzUiLCJjcmVhdG9ySWQiOiIxNDM2MyIsImlhdCI6MTcwMzU4MTAyOSwiZXhwIjo0NzgyODc3MDI5fQ.NCr2Cy9QsAHBvJ8OyLTNBYHeCW5gtkZY-5rA0gDk66y5hdEaC8bwP3aaOhgDWqT9BpPUhXO1GRGyXYeXKKnLow");
            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                CreateIssuerResponse createIssuerResponse = JsonConvert.DeserializeObject<CreateIssuerResponse>(response);
                return createIssuerResponse;
            }
            else
            {
                return new CreateIssuerResponse();
            }
        }
    }
}
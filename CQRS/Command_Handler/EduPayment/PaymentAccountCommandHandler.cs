using System.Text;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduShareCredential;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Newtonsoft.Json;

namespace Edu_Block_dev.CQRS.Command_Handler
{
    public class PaymentAccountCommandHandler : IRequestHandler<PaymentAccountCommand, ApiResponse<object>>
    {
        private readonly IConfiguration _configuration;
        private readonly EduBlockDataContext _context;

        public PaymentAccountCommandHandler(EduBlockDataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ApiResponse<object>> Handle(PaymentAccountCommand request, CancellationToken cancellationToken)
        {
            var nameParts = request.User.Name.Split(" ");
            var accountRequest = new AccountRequest
            {
                Account = new Account
                {
                    CrmKey = request.User.Id.ToString(),
                    Email = request.User.Email,
                    FirstName = request.User.Name.Split(" ")[0],
                    LastName = nameParts.Length >= 2 ? request.User.Name.Split(" ")[1] : string.Empty,
                    Phone = "",
                }
            };
            AccountResponse accountResponse = await this.PaymentAccountApiCall(accountRequest);
            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: accountResponse, message: "added permission");
        }

        public async Task<AccountResponse> PaymentAccountApiCall(AccountRequest accountRequest)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://app.shuttleglobal.com/c/api/instances/"+ _configuration.GetSection("Shuttle:InstanceId").Value + "/payment_url");

            var authKey = _configuration.GetSection("Shuttle:Authorization").Value;
            request.Headers.Add("Authorization", authKey);

            var json = JsonConvert.SerializeObject(accountRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<AccountResponse>(response);
            }
            else
            {
                return new AccountResponse();
            }
        }
    }
}
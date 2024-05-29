using System.Text;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduShareCredential;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Newtonsoft.Json;

namespace Edu_Block_dev.CQRS.Command_Handler
{
    public class PaymentUrlCommandHandler : IRequestHandler<PaymentUrlCommand, ApiResponse<object>>
    {
        private readonly IConfiguration _configuration;
        private readonly EduBlockDataContext _dbContext;
        private readonly IMediator _mediator;
        private readonly Util _util;

        public PaymentUrlCommandHandler(Util util, IConfiguration configuration, EduBlockDataContext dbContext, IMediator mediator)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _mediator = mediator;
            _util = util;
        }

        public async Task<ApiResponse<object>> Handle(PaymentUrlCommand request, CancellationToken cancellationToken)
        {
            var Amount = _dbContext.AvailableSubscriptions.Where(a => a.ID == request.PaymentUrlDTO.AvailableSubscriptionID).FirstOrDefault().Amount;
            var nameParts = request.User.Name.Split(" ");
            PaymentRequest paymentRequest = new PaymentRequest
            {
                Payment = new Payment
                {
                    Title = request.PaymentUrlDTO.Title,
                    Currency = request.PaymentUrlDTO.Currency,
                    Amount = Amount.ToString(),
                    SuccessUrl = _configuration.GetSection("Shuttle:Authorization").Value,
                    CancelUrl = _configuration.GetSection("Shuttle:Authorization").Value,
                    AltKey = Guid.NewGuid().ToString()+" "+ request.PaymentUrlDTO.AvailableSubscriptionID+ " " + request.User.Id,
                    Description = request.PaymentUrlDTO.Description,
                    Account = new Account
                    {
                        CrmKey = request.User.Id.ToString(),
                        FirstName = request.User.Name.Split(" ")[0] != null && request.User.Name.Split(" ")[0] != String.Empty ? request.User.Name.Split(" ")[0] : string.Empty,
                        LastName = nameParts.Length >= 2 ? request.User.Name.Split(" ")[1] : string.Empty,
                        Email = request.User.Email,
                        Phone = ""
                    }
                }
            };
            PaymentResponse paymentResponse = await this.PaymentUrlApiCall(paymentRequest);
            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: paymentResponse, message: "");
        }

        public async Task<AccountDTO> AccountDetailsApiCall(string accountId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://app.shuttleglobal.com/c/api/instances/" + _configuration.GetSection("Shuttle:InstanceId").Value + "/accounts/" + accountId);

            var authKey = _configuration.GetSection("Shuttle:Authorization").Value;
            request.Headers.Add("Authorization", authKey);

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                AccountDTO AccountDTO = JsonConvert.DeserializeObject<AccountDTO>(response);

                //PaymentOutputDTO PaymentOutputDTO = new PaymentOutputDTO()
                //{
                //    Account = test.payment.account,
                //    Amount = test.payment.amount,
                //    Action = test.payment.action,
                //    AltKey = test.payment.alt_key,
                //    created = test.payment.created,
                //    transaction = test.payment.transaction,
                //};

                //return PaymentOutputDTO;

                return AccountDTO;

            }
            else
            {
                return null;
            }
        }


        public async Task<PaymentResponse> PaymentUrlApiCall(PaymentRequest paymentRequest)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://app.shuttleglobal.com/c/api/instances/"+ _configuration.GetSection("Shuttle:InstanceId").Value + "/payment_url");

            var authKey = _configuration.GetSection("Shuttle:Authorization").Value;
            request.Headers.Add("Authorization", authKey);

            var json = JsonConvert.SerializeObject(paymentRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<PaymentResponse>(response);
            }
            else
            {
                return new PaymentResponse();
            }
        }


    }
}
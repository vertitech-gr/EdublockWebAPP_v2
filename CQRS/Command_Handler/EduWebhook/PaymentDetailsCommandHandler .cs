using AutoMapper;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduUserRequest;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Newtonsoft.Json;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUserRequest
{
    public class PaymentDetailsCommandHandler : IRequestHandler<PaymentDetailsCommand, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _dbContext;
        private readonly IRepository<PaymentDetail> _paymentDetailsRepository;
        private readonly IRepository<PaymentOutput> _paymentOutputRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentDetailsCommandHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly Util _util;

        public PaymentDetailsCommandHandler(Util util,IConfiguration configuration, IRepository<PaymentOutput> paymentOutputRepository, IRepository<PaymentDetail> paymentDetailsRepository,EduBlockDataContext dbContext, IMapper mapper, ILogger<PaymentDetailsCommandHandler> logger)
        {
            _dbContext = dbContext;
            _util = util;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _paymentDetailsRepository = paymentDetailsRepository;
            _paymentOutputRepository = paymentOutputRepository;
        }

        public async Task<ApiResponse<object>> Handle(PaymentDetailsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _paymentDetailsRepository.AddAsync(request.PaymentDetail);
                PaymentOutputDTO PaymentOutputDTO = await this.PaymentOutputApiCall(request.PaymentDetail.Payment);
                PaymentOutput paymentOutput = new PaymentOutput()
                {
                     Id = Guid.NewGuid(),
                     Account = PaymentOutputDTO.Account,
                     Action = PaymentOutputDTO.Action,
                     AltKey = PaymentOutputDTO.AltKey,
                     Amount = PaymentOutputDTO.Amount,
                     Balance = PaymentOutputDTO.Balance,
                     CreatedAt = PaymentOutputDTO.created,
                     transaction = PaymentOutputDTO.transaction
                };
                var paymentSubscription = PaymentOutputDTO.AltKey.Split(" ");
                Guid avlGuid = Guid.Parse(paymentSubscription[1]);
                Guid paymentSubsId = Guid.Parse(paymentSubscription[0]);


                AccountDTO AccountOutputDTO = await this.AccountDetailsApiCall(PaymentOutputDTO.Account);
                var subject = "Invoice EduBlock";
                var tableRows = "";
                tableRows += $@"
            <tr>
                <td>{paymentOutput.Id }</td>
                <td>{PaymentOutputDTO.created}</td>
                <td>${PaymentOutputDTO.Amount:F2}</td>
                <td>cards</td>
            </tr>";
                var content = $@"
                        <p>Dear Customer,</p>
                        <p>Please find your transaction details below:</p>
                        <table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse; width: 100%;'>
                            <thead>
                                <tr>
                                    <th>Transaction ID</th>
                                    <th>Date</th>
                                    <th>Amount</th>
                                    <th>Mode</th>
                                </tr>
                            </thead>
                            <tbody>
                                {tableRows}
                            </tbody>
                        </table>
                        <br>
                        <p>Thank you for using EduBlock!</p>
                        ";
                _util.SendSimpleMessage(subject, AccountOutputDTO.Account.Email, content, null, null);




                var payemtOutputResult = await _paymentOutputRepository.AddAsync(paymentOutput);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: result, message: "Request updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($" PaymentDetailsCommand payment webhook {ex.Message}");
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, data: null, message: "unable to update request");

            }
        }

        public async Task<PaymentOutputDTO> PaymentOutputApiCall(string paymentId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://app.shuttleglobal.com/c/api/instances/" + _configuration.GetSection("Shuttle:InstanceId").Value + "/payments/" + paymentId);

            var authKey = _configuration.GetSection("Shuttle:Authorization").Value;
            request.Headers.Add("Authorization", authKey);

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var test = JsonConvert.DeserializeObject<dynamic>(response);

                PaymentOutputDTO PaymentOutputDTO = new PaymentOutputDTO()
                {
                    Account = test.payment.account,
                    Amount = test.payment.amount,
                    Action = test.payment.action,
                    AltKey = test.payment.alt_key,
                    created = test.payment.created,
                    transaction = test.payment.transaction,
                };

                return PaymentOutputDTO;
            }
            else
            {
                return new PaymentOutputDTO();
            }
        }


        public async Task<object> TransactionDetailsApiCall(string transactionId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://app.shuttleglobal.com/c/api/instances/" + _configuration.GetSection("Shuttle:InstanceId").Value + "/transactions/" + transactionId);

            var authKey = _configuration.GetSection("Shuttle:Authorization").Value;
            request.Headers.Add("Authorization", authKey);

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var test = JsonConvert.DeserializeObject<dynamic>(response);
                return test;
            }
            else
            {
                return null;
            }
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
                return AccountDTO;
            }
            else
            {
                return null;
            }
        }
    }
}
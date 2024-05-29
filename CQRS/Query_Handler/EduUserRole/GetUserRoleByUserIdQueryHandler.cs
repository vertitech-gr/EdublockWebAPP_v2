using AutoMapper;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduUser;
using Edu_Block_dev.Modal.DTO;
using Newtonsoft.Json;
using Edu_Block.DAL.EF;
using System.Linq.Expressions;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetTransactionPaymentQueryHandler : IRequestHandler<GetPaymentTransactionQuery, ApiResponse<object>>
    {
        private readonly IRepository<PaymentDetail> _paymentDetailRepository;
        private readonly IRepository<PaymentOutput> _paymentOutputRepository;
        private readonly IRepository<AvailableSubscription> _availableSubscriptionRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly EduBlockDataContext _context;

        public GetTransactionPaymentQueryHandler(IRepository<PaymentOutput> paymentOutputRepository, IRepository<AvailableSubscription> availableSubscriptionRepository, EduBlockDataContext context, IRepository<PaymentDetail> paymentDetailRepository, IMapper mapper, IConfiguration configuration)
        {
            _paymentDetailRepository = paymentDetailRepository;
            _paymentOutputRepository = paymentOutputRepository;
            _mapper = mapper;
            _configuration = configuration;
            _context = context;
            _availableSubscriptionRepository = availableSubscriptionRepository;
        }

        public async Task<ApiResponse<object>> Handle(GetPaymentTransactionQuery request, CancellationToken cancellationToken)
        {
            try {
                List<PaymentOutput> PaymentOutputs = _context.PaymentOutputs.ToList();
                List<PaymentDetialsResponseDTO> PaymentDetialsResponses = new List<PaymentDetialsResponseDTO>();

                foreach (PaymentOutput PaymentOutput in PaymentOutputs)
                {
                    if (PaymentOutput != null)
                    {
                        var paymentSubscription = PaymentOutput.AltKey.Split(" ");
                        Guid avlGuid = Guid.Parse(paymentSubscription[1]);
                        AvailableSubscription availableSubscription = await _availableSubscriptionRepository.FindAsync(a => a.ID == avlGuid);

                        AccountDTO AccountOutputDTO = await this.AccountDetailsApiCall(PaymentOutput.Account);


                        PaymentDetialsResponseDTO paymentDetialsResponseDTO = new PaymentDetialsResponseDTO()
                        {
                            Credits = availableSubscription.Amount.ToString(),
                            Coins = availableSubscription.Coins.ToString(),
                            Mode = "Cards",
                            PlanName = availableSubscription.Name,
                            TransactionID = PaymentOutput.transaction,
                            PurchaseDate = PaymentOutput.CreatedAt,
                            Email = AccountOutputDTO.Account.Email
                        };
                        if (request.PaginationGuidDTO.guid != Guid.Empty)
                        {
                            if (request.PaginationGuidDTO.guid.ToString() == paymentSubscription[2])
                            {
                                PaymentDetialsResponses.Add(paymentDetialsResponseDTO);
                            }
                        }
                        else
                        {
                            PaymentDetialsResponses.Add(paymentDetialsResponseDTO);
                        }
                    }
                }

                var totalCount = PaymentDetialsResponses.Count();


                IQueryable<PaymentDetialsResponseDTO> QPaymentDetialsResponses = PaymentDetialsResponses.AsQueryable<PaymentDetialsResponseDTO>();

                if (!string.IsNullOrWhiteSpace(request.PaginationGuidDTO.SearchTerm))
                {
                    QPaymentDetialsResponses = QPaymentDetialsResponses.Where(p =>
                     p.PlanName.ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
                     p.TransactionID.ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
                     p.Coins.ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
                     p.Credits.ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
                     p.PurchaseDate.ToString().ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
                     ((string)p.Mode).ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()));
                }

                if (request.PaginationGuidDTO.SortOrder?.ToLower() == "desc")
                {
                    QPaymentDetialsResponses = QPaymentDetialsResponses.OrderByDescending(GetSortProperty(request));
                }
                else
                {
                    QPaymentDetialsResponses = QPaymentDetialsResponses.OrderBy(GetSortProperty(request));
                }

                QPaymentDetialsResponses = QPaymentDetialsResponses.Skip((request.PaginationGuidDTO.Page - 1) * request.PaginationGuidDTO.PageSize).Take(request.PaginationGuidDTO.PageSize);

                //var paymentDetailList = await PagedList<PaymentDetialsResponseDTO>.CreateAsync(
                //      QPaymentDetialsResponses,
                //      request.PaginationGuidDTO.Page,
                //      request.PaginationGuidDTO.PageSize);

                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new  { items = QPaymentDetialsResponses, request.PaginationGuidDTO.Page, request.PaginationGuidDTO.PageSize, totalCount, hasNextPage = (request.PaginationGuidDTO.Page * request.PaginationGuidDTO.PageSize < totalCount), hasPreviousPage = (request.PaginationGuidDTO.Page > 1) }, message: "Payment details done.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: null, message: "Payment details done.");
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

        public async Task<PaymentTransactionDto> PaymentTransactionApiCall(string transactionId)
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

                PaymentTransactionDto PaymentTransactionDto = new PaymentTransactionDto()
                {
                    Account = test.transaction.account,
                    Amount = test.transaction.amount,
                    Action = test.transaction.action,
                };

                return PaymentTransactionDto;
            }
            else
            {
                return new PaymentTransactionDto();
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

        private static Expression<Func<PaymentDetialsResponseDTO, object>> GetSortProperty(GetPaymentTransactionQuery request)
        {
            return request.PaginationGuidDTO.SortColumn?.ToLower() switch
            {
                "Mode" => avl => avl.Mode,
                "PlanName" => avl => avl.PlanName,
                _ => avl => avl.PurchaseDate
            };
        }

    }
}

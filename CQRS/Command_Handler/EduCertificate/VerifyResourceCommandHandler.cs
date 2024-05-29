using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using Edu_Block_dev.CQRS.Query.EduPurchaseSubscription;
using Edu_Block_dev.CQRS.Query.EduTransaction;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using System.Text;

namespace Edu_Block_dev.CQRS.Command_Handler
{
    public class VerifyResourceCommandHandler : IRequestHandler<VerifyResourceCommand, ShareVerificationResponse>
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Share> _shareRepository;
        private readonly IRepository<Envelope> _envelopeRepository;
        private readonly IRepository<Transactions> _transactionRepository;
        private readonly EduBlockDataContext _context;
        private readonly IConfiguration _configuration;

        public VerifyResourceCommandHandler(IRepository<Transactions> transactionRepository,EduBlockDataContext context, IConfiguration configuration, IRepository<Share> shareRepository, IRepository<Envelope> envelopeRepository, IMediator mediator)
        {
            _context = context;
            _configuration = configuration;
            _mediator = mediator;
            _shareRepository = shareRepository;
            _envelopeRepository = envelopeRepository; 
            _transactionRepository = transactionRepository;
        }

        public async Task<ShareVerificationResponse> Handle(VerifyResourceCommand request, CancellationToken cancellationToken)
        {
            bool verifiedStatus = true;
            decimal amountToDeduct = 0;
            int chargebleAmount = _configuration.GetValue<int>("PaymentConfig:CertificateCharge");

            Share share = await _shareRepository.FindAsync(u => u.Token == request._verifyResourceDTO.Token);

            if (share == null)
            {
                return new ShareVerificationResponse
                {
                    Status = VerificationStatus.Invalid,
                    Message = "Verification failed: Unauthorized"
                };
            }

            List<PurchaseSubscriptionResponseDTO> purchaseSubscriptions = await _mediator.Send(new PurchaseSubscriptionQuery(request.User.Id));
            PurchaseSubscriptionResponseDTO purchase = purchaseSubscriptions.Where(r => r.is_deleted == false && r.CoinBalance > 0).FirstOrDefault();

            if (purchase == null)
            {
                return new ShareVerificationResponse
                {
                    Status = VerificationStatus.subscription,
                    Message = "Please subscribe"
                };
            }

            var balanceDTO = await _mediator.Send(new GetBalanceQuery(request.User));

            if (balanceDTO.Amount <= 0)
            {
                return new ShareVerificationResponse
                {
                    Status = VerificationStatus.balance,
                    Message = "You have low balance. Please subscribe"
                };

            }

            if (request._verifyResourceDTO.Type == ShareType.Envelope)
            {
                ShareCertificateCredentialsDTO shareCredentialCertificates = _context.ShareCredentials
               .GroupJoin(_context.Certificates, sh => sh.CredentialId, c => c.Id, (sh, c) => new
               {
                   ShareCredential = sh,
                   Certificates = c.ToList()
               })
               .Where(sh => sh.ShareCredential.ShareId == share.Id)
               .Select(d => new ShareCertificateCredentialsDTO
               {
                   ShareCredential = d.ShareCredential,
                   Certificates = d.Certificates
               }).FirstOrDefault();

                shareCredentialCertificates.Certificates.ForEach(async (cre) =>
                {
                    if (!await this.VerifyCertificateApiCall(cre.CredentialsJson))
                    {
                        verifiedStatus = false;
                    }
                });
                amountToDeduct = shareCredentialCertificates.Certificates.Count * chargebleAmount;
            }

            if (request._verifyResourceDTO.Type == ShareType.Certificate)
            {
                var certificates = await _mediator.Send(new GetCertificatesQueryById(share.ResourceId));
                if (!await this.VerifyCertificateApiCall(certificates.CredentialsJson))
                {
                    return new ShareVerificationResponse
                    {
                        Status = VerificationStatus.IncompletePayment,
                        Message = "Please complete your payment.",
                        Amount = amountToDeduct
                    };
                }
                amountToDeduct = chargebleAmount;
            }

            Transactions transaction = await _transactionRepository.FindAsync(u => u.RefrenceId == share.Id);

            if (transaction == null)
            {
                return new ShareVerificationResponse
                {
                    Status = VerificationStatus.IncompletePayment,
                    Message = "Please complete your payment.",
                    Amount = amountToDeduct
                };
            }

            //if (share.Type == ShareType.Envelope)
            //{
            //    List<CertificateView> certificateDetails = await _mediator.Send(new EnvelopeRequestGroupQuery(share.ResourceId));
            //    amountToDeduct = certificateDetails.Count * chargebleAmount;
            //}

            //if (share.Type == ShareType.Certificate)
            //{
            //    amountToDeduct = chargebleAmount;
            //}

            Request sharedRequest = await _context.Requests.FindAsync(share.RequsetId);
            if (sharedRequest != null)
            {
                sharedRequest.Status = RequestStatus.Fulfilled;
                _context.Requests.Update(sharedRequest);
                await _context.SaveChangesAsync();
            }

            if (transaction != null && transaction.Status == TransactionStatus.Approved)
            {
                share.Verified = verifiedStatus;
                await _shareRepository.UpdateAsync(share.Id, share);
                return new ShareVerificationResponse
                {
                    Status = VerificationStatus.Valid,
                    Message = "Verification successful",
                    Share = share,
                    Amount = amountToDeduct
                };
            }

            if (transaction != null && transaction.Status != TransactionStatus.Approved)
            {
                return new ShareVerificationResponse
                {
                    Status = VerificationStatus.IncompletePayment,
                    Message = "Please complete your payment.",
                    Amount = amountToDeduct
                };
            }

            if (!verifiedStatus)
            {
                return new ShareVerificationResponse
                {
                    Status = VerificationStatus.IncompletePayment,
                    Message = "Please complete your payment.",
                    Amount = amountToDeduct
                };
            }
            else
            {
                return new ShareVerificationResponse
                {
                    Status = VerificationStatus.Valid,
                    Message = "Verification successful",
                    Share = share,
                    Amount = amountToDeduct
                };
            }
        }

        public async Task<bool> VerifyCertificateApiCall(string credentialJson)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-testnet.dock.io/verify");
            var apiKey = _configuration.GetSection("Dock:ApiKey").Value;
            request.Headers.Add("dock-api-token", apiKey);
            request.Headers.Add("authority", "api-testnet.dock.io");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("accept-language", "en-GB,en-US;q=0.9,en;q=0.8");
            request.Headers.Add("cache-control", "no-cache");
            var content = new StringContent(credentialJson, Encoding.UTF8, "application/json");
            request.Content = content;

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
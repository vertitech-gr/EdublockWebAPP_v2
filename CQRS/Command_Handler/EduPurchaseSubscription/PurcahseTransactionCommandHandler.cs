using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduPurchaseSubscription;
using Edu_Block_dev.CQRS.Commands.EduTransaction;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.CQRS.Query.EduTransaction;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Edu_Block_dev.CQRS.Command_Handler.EduPurchaseSubscription
{
    public class PurcahseTransactionCommandHandler : IRequestHandler<PurchaseCoinsCommand, Transactions>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public PurcahseTransactionCommandHandler(IConfiguration configuration, EduBlockDataContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
            _configuration = configuration;
        }

        public async Task<Transactions> Handle(PurchaseCoinsCommand request, CancellationToken cancellationToken)
        {
            CommanUser employer = request.User;
            BalanceDTO balanceDTO  = await _mediator.Send(new GetBalanceQuery(employer));
            if(balanceDTO.Amount <= 0)
            {
                throw new InvalidOperationException("Insufficient Balance");
            }
            int certificateCharge = _configuration.GetValue<int>("PaymentConfig:CertificateCharge");
            decimal tranactionAmount = 0;
            Share share = null ;
            Guid RefrenceId = Guid.Empty;
            if (TransactionType.Debit == request.TransactionType)
            {
                share = await _context.Shares.FirstOrDefaultAsync(u => u.Token == request.PurchaseCoinsDTO.token);
                if (share == null)
                {
                    throw new InvalidOperationException("Request doesn't exits");
                }
                if (share.Type == ShareType.Certificate)
                {
                    tranactionAmount = certificateCharge;
                }
                if (share.Type == ShareType.Envelope)
                {
                    List<CertificateView> certificates = await _mediator.Send(new EnvelopeRequestGroupQuery(share.ResourceId));
                    tranactionAmount = certificates.Count() * certificateCharge;
                }
                RefrenceId =  share.Id;
            }
            PurchaseSubscription purchaseSubscription = null;
            if (TransactionType.Credit == request.TransactionType)
            {
                purchaseSubscription = await _context.PurchaseSubscriptions.FindAsync(share.ResourceId);
                tranactionAmount = purchaseSubscription.CoinBalance;
            }

            TransactionDTO transactionDTO = new TransactionDTO()
            {
                Amount = tranactionAmount,
                PaymentID = 0,
                RefrenceId = RefrenceId,
                Status = TransactionStatus.Approved,
                Type = request.TransactionType,
                shareToken = (share == null) && share.Token == string.Empty ? "" : share.Token
            };
            Transactions transaction = await _mediator.Send(new TransactionCommand(transactionDTO, employer));
            return transaction;
        }

        
    }
}

using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduShareLink;
using Edu_Block_dev.CQRS.Query.EduPurchaseSubscription;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class ShareVerificationCommandHandler : IRequestHandler<ShareVerificationCommand, ShareVerificationResponse>
{
    private readonly EduBlockDataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;
    private readonly IRepository<Role> _roleRepository;

    public ShareVerificationCommandHandler(IRepository<Role> roleRepository,EduBlockDataContext context, IConfiguration configuration, IMediator mediator)
    {
        _context = context;
        _configuration = configuration;
        _mediator = mediator;
        _roleRepository = roleRepository;
    }
     
    public async Task<ShareVerificationResponse> Handle(ShareVerificationCommand request, CancellationToken cancellationToken)
    {
        decimal amountToDeduct = 0;
        int maxCount = _configuration.GetValue<int>("ShareConfig:MaxCount");
        int chargebleAmount = _configuration.GetValue<int>("PaymentConfig:CertificateCharge");
        var apiKey = _configuration.GetSection("Dock:ApiKey").Value;
        CommanUser user = request.User;

        Role role = await _roleRepository.FindAsync(r => r.NormalizedName == Edu_Block_dev.Authorization.Role.ADMIN.ToString());
        Role roleAdmin = await _roleRepository.FindAsync(r => r.NormalizedName == "ADMIN");

        Share share = await _context.Shares
            .SingleOrDefaultAsync(s =>
                s.Token == request.VerificationDTO.Token &&
                s.ReceiverId == user.Email,
                cancellationToken);

        if (role.Id == roleAdmin.Id)
        {
            share = await _context.Shares
            .SingleOrDefaultAsync(s =>
                s.Token == request.VerificationDTO.Token, 
                cancellationToken);

        }

        var result = await _mediator.Send(new PurchaseSubscriptionQuery(user.Id));

        if (share == null)
        {
            return new ShareVerificationResponse
            {
                Status = VerificationStatus.Invalid,
                Message = "Verification failed: Unauthorized"
            };
        }
        else if (share.ExpiryDate < DateTime.UtcNow)
        {
            return new ShareVerificationResponse
            {
                Status = VerificationStatus.Expiry,
                Message = "Verification failed: Link has expired"
            };
        }
        else if (share.Count >= maxCount)
        {
            return new ShareVerificationResponse
            {
                Status = VerificationStatus.Unauthorized,
                Message = "Verification failed: Maximum attempts exceeded"
            };
        }
        else
        {
            if(share.Type == ShareType.Envelope)
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

                amountToDeduct = shareCredentialCertificates.Certificates.Count * chargebleAmount;
            }

            if (share.Type == ShareType.Certificate)
            {
                amountToDeduct = chargebleAmount;
            }

            Transactions transaction = await _context.Transaction.FirstOrDefaultAsync(u => u.RefrenceId == share.Id);
            share.Count++; 
            await _context.SaveChangesAsync(cancellationToken);

            return new ShareVerificationResponse
            {
                Status = VerificationStatus.Valid,
                Message = "Verification successful",
                Share = share,
                Amount = amountToDeduct
            };
        }

    }
}

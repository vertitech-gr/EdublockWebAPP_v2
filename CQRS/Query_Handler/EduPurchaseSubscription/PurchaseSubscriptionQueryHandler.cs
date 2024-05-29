using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduPurchaseSubscription;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduPurchaseSubscription
{
    public class PurchaseSubscriptionQueryHandler : IRequestHandler<PurchaseSubscriptionQuery, List<PurchaseSubscriptionResponseDTO>>
    {
        private readonly EduBlockDataContext _context;
        private IRepository<UserProfile> _userProfileRepository;
        
        public PurchaseSubscriptionQueryHandler(EduBlockDataContext context, IRepository<UserProfile> userProfileRepository)
        {
            _context = context;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<List<PurchaseSubscriptionResponseDTO>> Handle(PurchaseSubscriptionQuery request, CancellationToken cancellationToken)
        {
            List<PurchaseSubscriptionResponseDTO> purchaseSubscriptions = new List<PurchaseSubscriptionResponseDTO>();
            if (request.Guid != Guid.Empty)
            {
                UserProfile employerProfile = await _userProfileRepository.FindAsync(up => up.UserID == request.Guid);
                purchaseSubscriptions = await _context.PurchaseSubscriptions.Where(p => p.UserProfileID == employerProfile.Id).Join( _context.AvailableSubscriptions, ps => ps.AvailableSubscriptionID, asb => asb.ID, (ps, asb) => new PurchaseSubscriptionResponseDTO
                {
                      AvailableSubscriptionID = ps.AvailableSubscriptionID,
                      CoinBalance = ps.CoinBalance,
                      CreatedAt = ps.CreatedAt,
                      created_by = ps.created_by,
                      EndDate = ps.EndDate,
                      Id = ps.Id,
                      is_deleted = ps.is_deleted,
                      is_deleted_at = ps.is_deleted_at,
                      Name = asb.Name,
                      StartDate = ps.StartDate,
                      UpdatedAt =  ps.UpdatedAt,
                      updated_by = ps.updated_by,
                      UserProfileID = ps.UserProfileID
                })
                .ToListAsync(cancellationToken);
            }
            else {
                purchaseSubscriptions = await _context.PurchaseSubscriptions.Join(_context.AvailableSubscriptions, ps => ps.AvailableSubscriptionID, asb => asb.ID, (ps, asb) => new PurchaseSubscriptionResponseDTO
                {
                    AvailableSubscriptionID = ps.AvailableSubscriptionID,
                    CoinBalance = ps.CoinBalance,
                    CreatedAt = ps.CreatedAt,
                    created_by = ps.created_by,
                    EndDate = ps.EndDate,
                    Id = ps.Id,
                    is_deleted = ps.is_deleted,
                    is_deleted_at = ps.is_deleted_at,
                    Name = asb.Name,
                    StartDate = ps.StartDate,
                    UpdatedAt = ps.UpdatedAt,
                    updated_by = ps.updated_by,
                    UserProfileID = ps.UserProfileID

                })
               .ToListAsync(cancellationToken);
            }
            return purchaseSubscriptions;
        }
    }
}
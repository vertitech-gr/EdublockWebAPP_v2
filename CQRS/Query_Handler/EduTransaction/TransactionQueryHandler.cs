using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduTransaction;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduTransaction
{
    public class TransactionQueryHandler : IRequestHandler<TransactionQuery, List<TransactionResponseDTO>>
    {
        private readonly EduBlockDataContext _context;

        public TransactionQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionResponseDTO>> Handle(TransactionQuery request, CancellationToken cancellationToken)
        {
            CommanUser employer = request.User;

            var result = await _context.Transaction
           .Where(p => p.UserProfileID == employer.UserProfile.Id )
           .Join(_context.Shares, transaction => transaction.RefrenceId, share => share.Id, (transaction, share) => new
                {
                    Transaction = transaction,
                    Share = share
                })
           .Join(_context.UserProfiles, share => share.Share.SenderId, userProfile => userProfile.Id, (share, userProfile) => new
                {
                    Transaction = share.Transaction,
                    Share = share.Share,
                    UserProfile = userProfile
                })
           .Join(_context.User, share => share.UserProfile.UserID, user => user.Id, (share, user) => new TransactionResponseDTO
                {
                    Transaction = share.Transaction,
                    Share = share.Share,
                    UserProfile = share.UserProfile,
                    User = user,
                })
            .ToListAsync(cancellationToken);
            return result.ToList();


        }

    }
}

using System.Linq.Expressions;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduTransaction;
using Edu_Block_dev.Helpers;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduTransaction
{
    public class AllTransactionQueryHandler : IRequestHandler<AllTransactionQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;

        public AllTransactionQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<object>> Handle(AllTransactionQuery request, CancellationToken cancellationToken)
        {
            CommanUser admin = request.User;
            var result = Enumerable.Empty<TransactionResponseDTO>();
            if (request.TransactionPaginationDTO.guid != Guid.Empty)
            {
                result = await _context.Transaction
               .Where(t => t.UserProfileID == admin.UserProfile.Id)
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
            }
            else
            {
               result = await _context.Transaction
              
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
            }
            IEnumerable<TransactionResponseDTO> enuTransactionResponses = result.ToList();
            IQueryable<TransactionResponseDTO> transactionResponses = enuTransactionResponses.AsQueryable<TransactionResponseDTO>();

            if (!string.IsNullOrWhiteSpace(request.TransactionPaginationDTO.SearchTerm))
            {
                transactionResponses = transactionResponses.Where(p =>
                 p.User.Name.ToLower().Contains(request.TransactionPaginationDTO.SearchTerm.ToLower()) ||
                 ((string)p.User.Email).ToLower().Contains(request.TransactionPaginationDTO.SearchTerm.ToLower())
                 );
            }

            if (request.TransactionPaginationDTO.SortOrder?.ToLower() == "desc")
            {
                transactionResponses = transactionResponses.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                transactionResponses = transactionResponses.OrderBy(GetSortProperty(request));
            }

            var employerTransResponseList = await PagedList<TransactionResponseDTO>.CreateAsync(
                  transactionResponses,
                  request.TransactionPaginationDTO.Page,
                  request.TransactionPaginationDTO.PageSize);

            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: employerTransResponseList, message: "Employer list");
        }

        private static Expression<Func<TransactionResponseDTO, object>> GetSortProperty(AllTransactionQuery request)
        {
            return request.TransactionPaginationDTO.SortColumn?.ToLower() switch
            {
                "name" => u => u.User.Name,
                "email" => u => u.User.Email,
                _ => u => u.User.Id
            };
        }

    }
}
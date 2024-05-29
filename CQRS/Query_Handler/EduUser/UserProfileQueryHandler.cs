using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduUser;
using MediatR;


namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class DetailsQueryHandler : IRequestHandler<DetailsQuery, UserProfile>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<UserProfile> _userRepository;

        public DetailsQueryHandler(EduBlockDataContext context, IRepository<UserProfile> userRepository)
        {
            _context = context;
            _userRepository = userRepository;

        }

        public async Task<UserProfile> Handle(DetailsQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.FindAsync(u => u.UserID == request._details.UserID);



        }
    }
}

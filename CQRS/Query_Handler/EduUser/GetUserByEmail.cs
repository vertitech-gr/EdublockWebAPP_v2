using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Query.EduUser;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, User>
    {
        private readonly IRepository<User> _userRepository;

        public GetUserByEmailQueryHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.FindAsync(u => u.Email == request.Email);
        }
    }
}

using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.CQRS.Query.EduUser;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class UserProfileCommandHandler : IRequestHandler<UserProfileCommand, UserProfile>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMediator _mediator;

        public UserProfileCommandHandler(EduBlockDataContext context, IMediator mediator)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<UserProfile> Handle(UserProfileCommand request, CancellationToken cancellationToken)
        {

            _context.UserProfiles.Add(request.userProfile);
            await _context.SaveChangesAsync();

            return request.userProfile;
        }
    }
}

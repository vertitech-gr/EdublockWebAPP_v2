using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduUser
{
    public class UserProfileCommand : IRequest<UserProfile>
    {
        public UserProfile userProfile;
        public UserProfileCommand(UserProfile userProfile)
        {
            this.userProfile = userProfile;
        }
    }
}

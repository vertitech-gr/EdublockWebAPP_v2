using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduUser
{
    public class GetUserProfileQuery : IRequest<UserProfile>
    {
        public UserProfile _userProfile;

        public GetUserProfileQuery(UserProfile userProfile)
        {
            _userProfile = userProfile;
        }
    }

    //public class GetUserProfileQueryById : IRequest<UserProfile>
    //{
    //    public Guid _userProfileId;

    //    public GetUserProfileQueryById(UserProfile userProfile)
    //    {
    //        _userProfile = userProfile;
    //    }
    //}

    public class DetailsQuery : IRequest<UserProfile>
    {
        public UserProfile _details;

        public DetailsQuery(UserProfile details)
        {
            _details = details;
        }
    }
}
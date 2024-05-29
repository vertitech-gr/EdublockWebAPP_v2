using System;
namespace EduBlock.Model.DTO
{
    public class UserWithProfile<T>
    {
        public T User { get; set; }
        public UserProfile Profile { get; set; }
        public DockIoDID Dock { get; set; }

        public UserWithProfile(T user, UserProfile profile, DockIoDID dock)
        {
            User = user;
            Profile = profile;
            Dock = dock;
        }

        public Guid GetProfileId()
        {
            return Profile.Id;
        }


        public string GetEmail()
        {
            return (User as dynamic).Email;
        }
    }

}


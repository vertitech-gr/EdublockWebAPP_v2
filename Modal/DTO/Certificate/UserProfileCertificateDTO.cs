using Edu_Block.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
  public class UserProfileCertificateDTO
    {
        public User User { get; set; }
        public UserProfile UserProfile { get; set; }
        public Certificate Certificate { get; set; }
    }
}
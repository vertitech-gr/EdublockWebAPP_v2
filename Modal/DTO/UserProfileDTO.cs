using Edu_Block.DAL.EF;
using Edu_Block_dev.DAL.EF;

namespace EduBlock.Model.DTO;

public class UserProfileDTO
{
    public User User { get; set; }
    public UserProfile Profile { get; set; }
}


public class TransactionResponseDTO
{
    public Transactions Transaction { get; set; }
   public Share Share { get; set; }
    public UserProfile UserProfile { get; set; }
    public User User { get; set; }
}


public class StudentDetailsDTO
{
    public User User { get; set; }
    public UserProfile UserProfile { get; set; }
    public University University { get; set; }
    public Department Department { get; set; }
    public DockIoDID? Dock { get; set; }
    public List<Certificate> Certificates { get; set; }
}

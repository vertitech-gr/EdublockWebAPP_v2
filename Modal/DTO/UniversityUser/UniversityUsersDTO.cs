
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
    public class UniversityUsersDTO
    {
        public Role Role { get; set; }
        public UniversityUser UniversityUser { get; set; }
        public Department Department { get; set; }
        public University University { get; set; }
    }
}
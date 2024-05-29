
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
    public class UniversityDockDetialsDTO
    {
        public University University { get; set; }
        public UserProfile UserProfile { get; set; }
        public DockIoDID DockIoDID { get; set; }
    }
}
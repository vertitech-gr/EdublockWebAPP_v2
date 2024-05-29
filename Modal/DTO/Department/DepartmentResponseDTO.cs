using Edu_Block.DAL.EF;
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
    public class DepartmentResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public bool Status { get; set; }
        public bool loginStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UniversityID { get; set; }
        public int requests { get; set; }
        public int students { get; set; }
        public UserRequest userRequest { get; set; }
        public User user { get; set; }
        public UniversityUser universityUser { get; set; }
       
    }
}
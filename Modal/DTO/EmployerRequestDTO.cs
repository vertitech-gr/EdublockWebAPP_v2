using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
    public class EmployerRequestDTO
    {
        public Request RequestItem { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}

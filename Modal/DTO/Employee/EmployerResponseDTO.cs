namespace Edu_Block_dev.Modal.DTO
{
    public class EmployerResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Industry { get; set; }
        public string SpecificIndustry { get; set; }
        public string PhoneNumber { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Did { get; set; } = string.Empty;
        public Guid EmployerProfileId { get; set; }
    }
}

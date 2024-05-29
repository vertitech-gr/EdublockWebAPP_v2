
namespace Edu_Block_dev.Modal.DTO
{
    public class UniversityResponseDTO
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? CountryCode { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Type { get; set; } = string.Empty;
        public bool? Active { get; set; }
        public bool? LoginStatus { get; set; }
        public bool? Status { get; set; }
        public string? did { get; set; } = string.Empty;
        public Guid? UserProfileId { get; set; } = Guid.Empty;
        public Guid? UserId { get; set; } = Guid.Empty;
        public DateTime? InviteDate { get; set; }
    }
}
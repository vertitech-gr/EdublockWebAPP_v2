using System;
namespace EduBlock.Model.DTO;


public class UserDetailsDTO
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid UniqueId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Status { get; set; }

}


namespace EduBlock.Model.DTO;

public class ChangePasswordDTO
{
    public string Key { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

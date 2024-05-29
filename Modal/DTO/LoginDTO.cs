namespace EduBlock.Model.DTO;

public enum LoginStatus
{
    Invalid,
    Initial,
    Completed
}

public class LoginDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
}

public class LoginGuidDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
}

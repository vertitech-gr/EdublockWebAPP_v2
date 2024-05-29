using Edu_Block.DAL.EF;

namespace EduBlock.Model.DTO;

public class UserDTO
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

}

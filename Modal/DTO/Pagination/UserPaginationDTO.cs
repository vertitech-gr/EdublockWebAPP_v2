
using Edu_Block_dev.Authorization;

namespace EduBlock.Model.DTO;

public class UserPaginationDTO: PaginationUniversityUserDTO
{
    public Role Type { get; set; }
}
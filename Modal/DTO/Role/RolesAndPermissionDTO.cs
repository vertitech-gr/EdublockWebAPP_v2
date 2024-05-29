using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
    public class RolesAndPermissionDTO
    {
       public Role Role { get; set; }
       public List<PermissionDetail> Permissions { get; set; }
       public List<RolePermissionMapping> RolePermissionMappings { get; set; }
    }

    public class RolesAndPermissionList
    {
        public Role Role { get; set; }
        public Permission Permissions { get; set; }
        public RolePermissionMapping RolePermissionMappings { get; set; }
    }
}
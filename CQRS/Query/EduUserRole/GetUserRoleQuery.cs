using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduUser
{
    public class GetUserRoleQuery : IRequest<UserRole>
    {
        public Guid _userId;
        public GetUserRoleQuery(Guid userId)
        {
            _userId = userId;
        }
    }

    public class GetRolesAndPermissionQuery : IRequest<RolesAndPermissionDTO>
    {
        public UserRole UserRole;
        public GetRolesAndPermissionQuery(UserRole userRole)
        {
            UserRole = userRole;
        }
    }
}

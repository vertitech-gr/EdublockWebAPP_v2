using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduAdmin;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.CQRS.Query.EduUniversity;
using Edu_Block_dev.CQRS.Query.EduUser;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.Authorization;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;


    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IJwtUtils jwtUtils, IMediator _mediator)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        UserJwtClaims? user = jwtUtils.ValidateJwtToken(token);

        CommanUser commanUser = new CommanUser();

        if (user != null && user.userId != null)
        {
            UserRole userRole = await _mediator.Send(new GetUserRoleQuery(new Guid(user.userId)));
            RolesAndPermissionDTO RolesAndPermissionDTO = new RolesAndPermissionDTO();
            if (userRole != null)
            {
                RolesAndPermissionDTO = await _mediator.Send(new GetRolesAndPermissionQuery(userRole));
            }

            if (userRole.UserRoleId == Role.EMPLOYER)
            {
                UserWithProfile<Employer> result = await _mediator.Send(new GetEmployeeByUniqueIdQuery(new Guid(user.userId)));
                commanUser = new CommanUser()
                {
                    Email = result.User.Email,
                    UserProfile = result.Profile,
                    Name = result.User.Name,
                    RolesAndPermissionDTO = RolesAndPermissionDTO,
                    Password = result.User.Password,
                    CreatedAt = result.User.CreatedAt,
                    Id = result.User.Id,
                    Status = result.User.Status,
                    LoginStatus = result.User.loginStatus,
                    UpdatedAt = result.User.UpdatedAt,
                    UserRole = userRole,
                    DID = result.Dock.DID,
                    Mode = user.mode,
                };
                context.Items["User"] = commanUser;
                context.Items["RoleId"] = userRole.UserRoleId;
            }
            else if(userRole.UserRoleId == Role.STUDENT)
            {
                UserWithProfile<User> result = await _mediator.Send(new GetUserByUniqueIdQuery(new Guid(user.userId)));
                commanUser = new CommanUser()
                {
                    Email = result.User.Email,
                    UserProfile = result.Profile,
                    Name = result.User.Name,
                    RolesAndPermissionDTO = RolesAndPermissionDTO,
                    Password = result.User.Password,
                    CreatedAt = result.User.CreatedAt,
                    Id = result.User.Id,
                    Status = result.User.Status,
                    LoginStatus = result.User.loginStatus,
                    UpdatedAt = result.User.UpdatedAt,
                    UserRole = userRole,
                    DID = result.Dock.DID
                };
                context.Items["User"] = commanUser;
                context.Items["RoleId"] = userRole.UserRoleId;

            }
            else if (userRole.UserRoleId == Role.UNIVERSITY)
            {
               UserWithProfile<University> result = await _mediator.Send(new GetUniversityByUniqueIdQuery(new Guid(user.userId)));
                commanUser = new CommanUser()
                {
                    Email = result.User.Email,
                    UserProfile = result.Profile,
                    Name = result.User.Name,
                    RolesAndPermissionDTO = RolesAndPermissionDTO,
                    Password = result.User.Password,
                    CreatedAt = result.User.CreatedAt,
                    Id = result.User.Id,
                    Status = result.User.Status,
                    LoginStatus = result.User.loginStatus,
                    UpdatedAt = result.User.UpdatedAt,
                    UserRole = userRole,
                    DID = result.Dock.DID
                };
                context.Items["User"] = commanUser;
                context.Items["RoleId"] = userRole.UserRoleId;

            }
            else if (userRole.UserRoleId == Role.ADMIN)
            {
                UserWithProfile<Admin> result = await _mediator.Send(new GetAdminByUniqueIdQuery(new Guid(user.userId)));
                commanUser = new CommanUser()
                {
                    Email = result.User.Email,
                    UserProfile = result.Profile,
                    Name = result.User.Name,
                    RolesAndPermissionDTO = RolesAndPermissionDTO,
                    Password = result.User.Password,
                    CreatedAt = result.User.CreatedAt,
                    Id = result.User.Id,
                    Status = result.User.Status,
                    LoginStatus = result.User.loginStatus,
                    UpdatedAt = result.User.UpdatedAt,
                    UserRole = userRole,
                    DID = result.Dock.DID
                };
                context.Items["User"] = commanUser;
                context.Items["RoleId"] = userRole.UserRoleId;
            }
            else if (userRole.UserRoleId == Role.UNIVERSITYUSER)
            {
                UserWithProfile<UniversityUser> result = await _mediator.Send(new GetUniversityUserByUniqueIdQuery(new Guid(user.userId)));
                UniversityResponseDTO _universityResponseDTO = await _mediator.Send(new GetUniversityByUniversityUserIdQuery(new Guid(user.userId)));
                commanUser = new CommanUser()
                {
                    Email = result.User.Email,
                    UserProfile = result.Profile,
                    Name = result.User.Name,
                    RolesAndPermissionDTO = RolesAndPermissionDTO,
                    Password = result.User.Password,
                    CreatedAt = result.User.CreatedAt,
                    Id = result.User.Id,
                    Status = result.User.Status,
                    LoginStatus = result.User.loginStatus,
                    UpdatedAt = result.User.UpdatedAt,
                    UserRole = userRole,
                    universityResponseDTO = _universityResponseDTO,
                    DID = result.Dock.DID
                };
                context.Items["User"] = commanUser;
                context.Items["RoleId"] = userRole.UserRoleId;
            }
        }
        await _next(context);
    }
}
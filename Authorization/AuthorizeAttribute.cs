using Edu_Block_dev.Modal.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Edu_Block_dev.Authorization
{
    public enum Role
    {
        ADMIN,
        STUDENT,
        EMPLOYER,
        UNIVERSITY,
        DEPARTMENT,
        SECRETARIAT,
        UNIVERSITYUSER
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public String Module { get; set; }
        public String Method { get; set; }
        public Boolean? Exposed { get; set; }

        public AuthorizeAttribute(String _module, String _method, Boolean _exposed = false)
        {
            Module = _module;
            Method = _method;
            Exposed = _exposed;
        }
     
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;
            CommanUser user = context.HttpContext.Items["User"] as CommanUser;
            if(user != null)
            {
                if (!user.LoginStatus)
                {
                    context.Result = new JsonResult(new { message = "Your account has been disabled, please contact admin." }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                if (user.RolesAndPermissionDTO == null)
                {
                    context.Result = new JsonResult(new { message = "You are not authorized for this action." }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else
                {
                    var result = user.RolesAndPermissionDTO.Permissions.Where(p => p.Module.ToUpper() == Module.ToUpper() && p.Method.ToUpper() == Method.ToUpper()).ToList();
                    if (Exposed != null && ((Exposed == false && user.Mode == "api")))
                    {
                        context.Result = new JsonResult(new { message = "You are not authorized for this action." }) { StatusCode = StatusCodes.Status401Unauthorized };
                    }
                    else
                    {
                        if (result != null && result.Count() <= 0 )
                        {
                            context.Result = new JsonResult(new { message = "You are not authorized for this action." }) { StatusCode = StatusCodes.Status401Unauthorized };
                        }
                    }
                }
            }
            else
            {
                context.Result = new JsonResult(new { message = "You are not authorized for this action." }) { StatusCode = StatusCodes.Status401Unauthorized };

            }

         
        }
    }
}
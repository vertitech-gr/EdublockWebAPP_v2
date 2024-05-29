using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth;
using Edu_Block_dev.Modal.DTO;
using Facebook;
using Microsoft.AspNetCore.Authorization;

namespace Edu_Block_dev.Controllers
{   
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("google")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequest request)
        {
            try
            {
               
                var validPayload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);

                var userId = validPayload.Subject;
                var userEmail = validPayload.Email;
                var userName = validPayload.Name;

                return Ok(new { Message = "Successfully authenticated with Google" });
            }
            catch (InvalidJwtException ex)
            {
                return BadRequest(new { Message = "Invalid Google ID token" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }


        [HttpPost("facebook")]
        public IActionResult FacebookSignIn([FromBody] FacebookSignInRequest request, [FromServices] IConfiguration configuration)
        {
            try
            {

                var facebookAuthConfig = configuration.GetSection("FacebookAuth");
                var facebookAppId = facebookAuthConfig["AppId"];
                var facebookAppSecret = facebookAuthConfig["AppSecret"];

                var fb = new FacebookClient();

                dynamic result = fb.Get("debug_token", new
                {
                    input_token = request.AccessToken,
                    access_token = $"{facebookAppId}|{facebookAppSecret}"
                });

                if (result.data.is_valid != true)
                {
                    return BadRequest(new { Message = "Invalid Facebook access token." });
                }

                var userId = result.data.user_id;
                var userEmail = result.data.email;


                return Ok(new { Message = "Successfully authenticated with Facebook" });
            }
            catch (InvalidJwtException ex)
            {
                return BadRequest(new { Message = "Invalid Facebook access token" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }


    }


}

using Edu_Block.DAL.EF;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using Edu_Block_dev.CQRS.Query.EduUniversity;
using Edu_Block_dev.Authorization;
using EduBlock.Model.DTO;
using Edu_Block_dev.Modal.DTO;
using AutoMapper;

namespace Edu_Block_dev.Controllers.EduUniversity
{
    [ApiController]
    [Route("api/universities")]
    public class UniversityController : ControllerBase
    {
        private readonly EduBlockDataContext _context;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UniversityController(EduBlockDataContext context, IMediator mediator, IMapper mapper)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("create-universities")]
        [Authorize("UNIVERSITIES","CREATE")]
        public async Task<IActionResult> CreateUniversity([FromForm] UniversityRequestDTO UniversityRequestDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new CreateUniversityCommand(UniversityRequestDTO, user));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("resend-universities")]
        [Authorize("UNIVERSITIES","RESEND")]
        public async Task<IActionResult> ResendUniversity(Guid guid)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new ResendUniversityCommand(guid, user));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("verify-university-registration")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyUniversityEmail([FromBody] VerifyEmailGuidDTO VerifyEmailGuidDTO)
        {
            var result = await _mediator.Send(new VerifyUniversityEmailCommand(VerifyEmailGuidDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("forget-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetEmployeePassword([FromBody] EmailRoleGuidDTO emailRoleGuidDTO)
        {
            var result = await _mediator.Send(new ForgetUniversityPasswordCommand(emailRoleGuidDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }
       
        [HttpPost]
        [Route("university-login")]
        [AllowAnonymous]
        public async Task<IActionResult> UniversityLogin([FromBody] LoginGuidDTO universityLoginDTO)
        {
            var result = await _mediator.Send(new UniversityLoginCommand(universityLoginDTO));
            return result.Success ?  Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("change-university-password")]
        [Authorize("UNIVERSITIES","CHANGE_PASSWORD")]
        public async Task<IActionResult> ChangeUniversityPassword(string newPassword, Guid RoleId)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new ChangeUniversityPasswordCommand(user, newPassword, RoleId));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("resend-university-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendUniversityOtp(string Email, Guid RoleId)
        {
            var result = await _mediator.Send(new ResendUniversityOtpCommand(Email, RoleId));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-universities")]
        public async Task<IActionResult> GetUniversities([FromQuery] UniversityPaginationDTO UniversityPaginationDTO)
        {
            var result = await _mediator.Send(new GetUniversitiesQuery(UniversityPaginationDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-universities-by-student")]
        public async Task<IActionResult> GetUniversitiesByStudent([FromQuery] UniversityPaginationDTO UniversityPaginationDTO)
        {
            var result = await _mediator.Send(new GetUniversitiesByStudentQuery(UniversityPaginationDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-universities/{id}")]
        public async Task<ActionResult<University>> GetUniversity(Guid id)
        {
            UserWithProfile<University> result = await _mediator.Send(new GetUniversityByUniqueIdQuery(id));
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        
        [HttpPatch("update-university")]
        [Authorize("UNIVERSITIES","UPDATE")]
        public async Task<IActionResult> UpdateUniversity([FromForm] Guid id, [FromForm] UniversityUpdateRequestDTO universityRequestDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new UpdateUniversityCommand(user, id, universityRequestDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete-university/{id}")]
        public async Task<IActionResult> DeleteUniversity(Guid id)
        {
            var university = await _context.Universities.FindAsync(id);
            if (university == null)
            {
                return NotFound();
            }
            _context.Universities.Remove(university);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("university-details")]
        [Authorize("UNIVERSITIES","GET")]
        public async Task<IActionResult> GetUniversityDetails()
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            if (user != null)
            {
                return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: user, message: "University details retrieved successfully"));
            }
            return NotFound("University not found");
        }
    }
}
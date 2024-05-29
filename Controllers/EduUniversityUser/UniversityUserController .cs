using Edu_Block.DAL.EF;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using Edu_Block_dev.Modal.DTO;
using AutoMapper;
using Edu_Block_dev.Authorization;
using EduBlock.Model.DTO;
using Edu_Block_dev.CQRS.Query.EduAdmin;
using Edu_Block.DAL;

namespace Edu_Block_dev.Controllers.EduUniversity
{
    [ApiController]
    [Route("api/universities")]
    public class UniversityUserController : ControllerBase
    {
        private readonly EduBlockDataContext _context;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public UniversityUserController(EduBlockDataContext context, IMediator mediator, IMapper mapper, Util util)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
            _util = util;
        }

        [HttpPost]
        [Route("create-university-user")]
        public async Task<IActionResult> CreateUniversityUser([FromBody] UniversityUserRequestDTO UniversityUserRequestDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new CreateUniversityUserCommand(UniversityUserRequestDTO, user));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("all-University-user-list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTransactions([FromQuery] PaginationUniversityUserDTO PaginationUniversityUserDTO)
        {
            CommanUser user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new NewListUniversityUserQuery(user, PaginationUniversityUserDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("update-login-status")]
        [Authorize("UNIVERSITY_USER", "LOGIN_STATUS")]
        public async Task<IActionResult> UpdateLoginStatus(LoginStatusDTO LoginStatusDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new UpdateLoginStatusCommand(LoginStatusDTO, user));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Commands.EmployeeRequest;
using Edu_Block_dev.CQRS.Query.EduEmployeeRequest;
using Edu_Block_dev.CQRS.Query.EmployeeRequest;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Block_dev.Controllers.EmployeeRequest
{
    [ApiController]
    [Route("api")]
    public class EmployeeRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("employer-requests")]
        [Authorize("EmployeeRequest","LIST")]
        public async Task<ActionResult<List<Request>>> GetEmployerRequests([FromQuery] PaginationStatusGuidDTO PaginationStatusGuidDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var envelopes = await _mediator.Send(new GetEmployerRq(user, PaginationStatusGuidDTO));
            return Ok(envelopes);
        }

        [HttpGet("received-request")]
        [Authorize("EmployeeRequest","GET")]
        public async Task<IActionResult> GetEmployeeRequest([FromQuery] PaginationGuidDTO paginationGuidDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var query = new EmployeeRequestQuery(paginationGuidDTO, new EmployeeResponseDTO { EmpName = user.Name, ReceiverId = user.UserProfile.Id },  user);
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("create-employee-request")]
        [Authorize("EmployeeRequest","CREATE")]
        public async Task<IActionResult> CreateEmployeeRequest([FromBody] EmployeeRequestDTO requestDto)
        {
            CommanUser? employer = HttpContext.Items["User"] as CommanUser;
            var createCommand = new EmployeeRequestCommand(requestDto, employer);
            var createdDto = await _mediator.Send(createCommand);
            return Ok(createdDto);
        }

        [HttpPost]
        [Route("employee-request-group")]
        public async Task<IActionResult> EmployeeRequestGroup([FromBody] EmployeeRequestGroupDTO requestGroupDTO)
        {
            var createCommand = new EmployeeRequestGroupCommand(requestGroupDTO);
            var createdDto = await _mediator.Send(createCommand);
            return Ok(createdDto);
        }

        [HttpPatch("reject/{requestId}")]
        [Authorize("EmployeeRequest","REJECT")]
        public async Task<IActionResult> RejectRequest(Guid requestId)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var command = new RejectRequestCommand(requestId, user.UserProfile.Id);
            var result = await _mediator.Send(command);
            if (result != null)
            {
                return new OkObjectResult(new ApiResponse<object>( System.Net.HttpStatusCode.OK, message: "Request reject successfully"));
            }
            else
            {
                return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError , message: "Request not reject successfully"));
            }
        }

    }
}

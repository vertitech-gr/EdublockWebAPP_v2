using Edu_Block_dev.CQRS.Commands.EduUserRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Edu_Block_dev.Modal.DTO;
using AutoMapper;
using Edu_Block_dev.CQRS.Query.EduUserRequest;
using EduBlock.Model.DTO;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Commands.EduRole;

namespace Edu_Block_dev.Controllers.EduUserRequest
{
    [ApiController]
    [Route("api/user-requests")]
    public class RequestsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public RequestsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("create")]
        [Authorize("USER_REQUEST","CREATE")]
        public async Task<IActionResult> CreateRequest([FromBody] UserRequestDto UserRequestDto)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new CreateUserRequestCommand(user, UserRequestDto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        [Route("recieved-request")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRequests([FromQuery] PaginationReceivedRequestDTO PaginationReceivedRequestDTO)
        {
           var result = await _mediator.Send(new GetRequestsQuery(PaginationReceivedRequestDTO));
           return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        [Route("outgoing-request")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOutgoingRequests([FromQuery] PaginationGuidDTO PaginationGuidDTO)
        {
            var result = await _mediator.Send(new GetOutgoingRequestsQuery(PaginationGuidDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("create-request-message")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateRequestMessage([FromForm] RequestMessageDTO RequestMessageDTO)
        {
            var result = await _mediator.Send(new RequestMessageCommand(RequestMessageDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        [Route("get-request-message")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRequestMessage([FromQuery] TypedPaginationGuidDTO typedPaginationGuidDTO)
        {
            var result = await _mediator.Send(new GetRequestMessageQuery(typedPaginationGuidDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("request-status")]
        [Authorize("USER_REQUEST", "STATUS")]
        public async Task<IActionResult> UpdateRequestStatus(Guid guid, MessageStatus status)
        {
            var result = await _mediator.Send(new UpdateRequestStatusCommand(guid, status));
            return result.Success ? Ok(result) : BadRequest(result);
        }


    }
}

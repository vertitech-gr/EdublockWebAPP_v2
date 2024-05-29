using Edu_Block.DAL.EF;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Commands.EduEnvelope;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.Modal.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Block_dev.Controllers.EduEnvelope
{
    [ApiController]
    [Route("api/envelopes")]
    public class EnvelopeController : ControllerBase
    {
        private readonly EduBlockDataContext _context;
        private readonly IMediator _mediator;

        public EnvelopeController(EduBlockDataContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [HttpGet("get-envelope-by-order/{sortBy}/{orderBy}/{type}")]
        [Authorize("ENVELOPE","LIST")]
        public async Task<ActionResult<List<Envelope>>> GetEnvelopesByOrder(string sortBy, SortingOrder orderBy, EnvelopeShareType type )
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var query = new AllEnvelopeQuery(user.UserProfile.Id, sortBy, orderBy, type);
            var envelopes = await _mediator.Send(query);
            return Ok(envelopes);
        }

        [HttpPost("create")]
        [Authorize("ENVELOPE","CREATE")]
        public async Task<IActionResult> CreateEnvelope([FromBody] EnvelopeDTOC envelopeDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new EnvelopeCommand(envelopeDTO, user, EnvelopeShareType.CREATE));
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost("share-envelope")]
        [Authorize("ENVELOPE","SHARE")]
        public async Task<IActionResult> ShareEnvelope([FromBody] EnvelopeCreationDto envelopeCreation)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var envelope = await _mediator.Send(new ShareEnvelopeByIdQuery( user.UserProfile.Id, envelopeCreation.EnvelopID, envelopeCreation.Credentials, envelopeCreation.Email));
            if (envelope == null)
                return NotFound();
            return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, message: "Envelope shared successfully"));
        }

        [HttpPost("share-envelope-url")]
        [Authorize("ENVELOPE","SHARE_URL")]
        public async Task<IActionResult> ShareEnvelopeUrl([FromBody] EnvelopeUrlDto envelopeUrlDto)
        {
            var url = await _mediator.Send(new ShareEnvelopeUrlQuery(envelopeUrlDto.EnvelopID));
            if (url == null)
                return NotFound();
            return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: url, message: "Envelope shared successfully"));
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<EnvelopResponseDTO>> GetEnvelopeById(Guid id)
        {
            EnvelopResponseDTO _envelope = await _mediator.Send(new EnvelopQueryById(id));
            if (_envelope == null)
                return NotFound();
            return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: _envelope, message: "Get Envelope details successfully"));
        }

        [HttpGet("getShare")]
        public async Task<ActionResult<EnvelopResponseDTO>> GetSharedEnvelopeById(string Token)
        {
            EnvelopResponseDTO _envelope = await _mediator.Send(new SahredEnvelopQueryById(Token));
            if (_envelope == null)
                return NotFound();
            return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: _envelope, message: "Get Envelope details successfully"));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Envelope(Guid id)
        {
            var Envelope = await _context.Envelopes.FindAsync(id);
            if (Envelope == null)
            {
                return NotFound();
            }
            _context.Envelopes.Remove(Envelope);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("update-group")]
        public async Task<IActionResult> UpdateEnvelopeGroup([FromBody] EnvelopeGroupDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid command");
            }
            var command = new UpdateEnvelopeGroupCommand( dto);
            var result = await _mediator.Send(command);
            if (result != null)
            {
                return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, message: "Envelope Update successfully"));
            }
            return NotFound();
        }

        private bool EnvelopeExists(Guid id)
        {
            return _context.Envelopes.Any(e => e.Id == id);
        }

    }

}

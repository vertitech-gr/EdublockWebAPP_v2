using Microsoft.AspNetCore.Mvc;
using MediatR;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.CQRS.Commands.EduShareCredential;
using Edu_Block_dev.CQRS.Query.EduShareCredentialQuery;

namespace Edu_Block_dev.Controllers.EduShareCredential
{
    [ApiController]
    [Route("api/credential")]
    public class CredentialController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CredentialController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("create")]
        public async Task<ActionResult<int>> ShareResource([FromBody] ShareCredentialDTO dto)
        {
            var result = await _mediator.Send(new ShareCredentialCommand(dto));
            return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK ,data: result, message: " Credential Shared successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShareCredentialDTO>> GetShareCredentialById(Guid id)
        {
            var query = new ShareCredentialQuery { Id = id };
            var shareCredential = await _mediator.Send(query);

            if (shareCredential == null)
            {
                return NotFound();
            }

            return Ok(shareCredential);
        }

    }

}

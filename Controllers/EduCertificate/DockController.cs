using Microsoft.AspNetCore.Mvc;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using MediatR;
using Edu_Block_dev.Modal.Dock;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using EduBlock.Model.DTO;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Commands.EduCertificate;
using Edu_Block_dev.Modal.DTO;

namespace Edu_Block_dev.Controllers.EduCertificate
{
    [Route("api/dock")]
    [ApiController]
    public class DockController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DockController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-holder")]
        public async Task<IActionResult> CreateHolder()
        {
            var result = await _mediator.Send(new CreateDockHandlerCommand());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("create-issuer")]
        public async Task<IActionResult> CreateIssuer(IssuerDTO issuerDTO)
        {
            var result = await _mediator.Send(new CreateDockIssuerCommand(issuerDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("create-certificate")]
        public async Task<IActionResult> CreateCredential(IssuerCredentialRequest issuerCredentialRequest)
        {
            var result = await _mediator.Send(new IssuerCredentialCommand(issuerCredentialRequest));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("revoke-certificate")]
        public async Task<IActionResult> RevokeCertificate(Guid guid)
        {
            var result = await _mediator.Send(new RevokeCredentialCommand(guid));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("resend-certificate")]
        public async Task<IActionResult> ResendCertificate(Guid guid)
        {
            var result = await _mediator.Send(new ResendCredentialCommand(guid));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-certificate")]
        [Authorize("CREDENTIAL", "GetCertificate")]
        public async Task<IActionResult> GetCertificates()
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new GetCertificatesQuery(user));
            return Ok(result);
        }

        [HttpGet("get-certificates")]
        [Authorize("CREDENTIAL", "GetCertificates")]
        public async Task<IActionResult> GetCertificateResponse([FromQuery] PaginationUniversityUserDTO PaginationUniversityUserDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new GetCertificateResponseQuery(user, PaginationUniversityUserDTO));
            return Ok(result);
        }

        [HttpPost("get-certificate-by-id")]
        public async Task<IActionResult> GetCertificatesByID(Guid certificateId)
        {
            var certificates = await _mediator.Send(new GetCertificatesQueryById(certificateId));
            if (certificates == null)
            {
                return NotFound("No certificates found");
            }
            return Ok(certificates);
        }

       
        [HttpPost("verify-resource-by-token")]
        [Authorize("CREDENTIAL", "VerifyResourceById")]
        public async Task<IActionResult> VerifyResourceById(VerifyResourceDTO verifyResourceDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new VerifyResourceCommand(verifyResourceDTO, user));
            if (result.Status == VerificationStatus.Valid)
            {
                return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK ,data: result,  message: "Resource verified"));
            }
            return new OkObjectResult(new ApiResponse<object>( System.Net.HttpStatusCode.InternalServerError ,data: result, message: "Resource unverified"));
        }
        [HttpGet("get-certificate-url")]
        public async Task<IActionResult> GetCertificateUrlQuery(Guid certificateId)
        {
            var responseData = await _mediator.Send(new GetCertificateUrlQuery(certificateId));
            if (responseData == null)
            {
                return NotFound("No certificates found");
            }
            return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: responseData, message: "Envelope shared successfully"));
        }

        [HttpPost("share")]
        public async Task<IActionResult> ShareCertificate([FromBody] ShareCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
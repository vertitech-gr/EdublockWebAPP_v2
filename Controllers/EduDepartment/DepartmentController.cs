using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Commands.EduDepartment;
using Edu_Block_dev.CQRS.Query.EduDepartment;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/department")]
public class DepartmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepartmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetDepartments([FromQuery] PaginationUniversityGuidDTO PaginationUniversityGuidDTO)
    {
        CommanUser? user = HttpContext.Items["User"] as CommanUser;
        var result = await _mediator.Send(new DepartmentQuery(user, PaginationUniversityGuidDTO));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-department-by-university-id")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDepartmentByUniversityId([FromQuery] PaginationGuidDTO paginationGuidDTO)
    {
        var result = await _mediator.Send(new DepartmentQueryByUniversityId(paginationGuidDTO));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-by-university-user/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDepartmentByUniversityUserId(Guid id)
    {
        var result = await _mediator.Send(new DepartmentQueryByUniversityUserId(id));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("create-department")]
    [Authorize("DEPARTMENT", "Create")]
    public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDTO department)
    {
        CommanUser? user = HttpContext.Items["User"] as CommanUser;
        var result = await _mediator.Send(new CreateDepartmentCommand(user, department));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("update-department")]
    [Authorize("DEPARTMENT", "Update")]
    public async Task<IActionResult> UpdateDepartment(DepartmentRequestDTO departmentRequestDTO)
    {
        CommanUser? user = HttpContext.Items["User"] as CommanUser;
        var result = await _mediator.Send(new UpdateDepartmentCommand(user, departmentRequestDTO));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("delete-department")]
    [Authorize("DEPARTMENT", "Delete")]
    public async Task<IActionResult> deleteDepartment(Guid guid)
    {
        CommanUser? user = HttpContext.Items["User"] as CommanUser;
        var result = await _mediator.Send(new DeleteDepartmentCommand(user, guid));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("upload-student")]
    public async Task<IActionResult> UploadStudent([FromForm] UploadStudentRequestDTO uploadStudentRequestDTO)
    {
        var result = await _mediator.Send(new UploadStudentCommand(uploadStudentRequestDTO));
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
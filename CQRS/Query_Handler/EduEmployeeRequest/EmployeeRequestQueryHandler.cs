using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduEmployer;
using Edu_Block_dev.CQRS.Query.EmployeeRequest;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;


public class EmployeeRequestQueryHandler : IRequestHandler<EmployeeRequestQuery, ApiResponse<object>>
{
    private readonly EduBlockDataContext _context;
    private readonly IMediator _mediator;

    public EmployeeRequestQueryHandler(EduBlockDataContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<ApiResponse<object>> Handle(EmployeeRequestQuery request, CancellationToken cancellationToken)
    {

    try {

        var receivedId = request.employeeResponseDTO.ReceiverId;
        var totalCount = _context.Requests.Count();
        var employeeRequests = await _context.Requests
            //.Skip((request.PaginationGuidDTO.Page - 1) * request.PaginationGuidDTO.PageSize).Take(request.PaginationGuidDTO.PageSize)
            .Where(r => r.ReceiverId == receivedId)
            .ToListAsync(cancellationToken);

        if(request.CommanUser.RolesAndPermissionDTO.Role.NormalizedName == "ADMIN")
        {
            employeeRequests = await _context.Requests
            //.Skip((request.PaginationGuidDTO.Page - 1) * request.PaginationGuidDTO.PageSize).Take(request.PaginationGuidDTO.PageSize)
            .ToListAsync(cancellationToken);
        }

        if (employeeRequests == null || !employeeRequests.Any())
        {
            return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, data: null, message: "No list founond");
        }

        List<EmployeeResponseDTO> responseList = new List<EmployeeResponseDTO>();


            employeeRequests = employeeRequests.Skip((request.PaginationGuidDTO.Page - 1) * request.PaginationGuidDTO.PageSize).Take(request.PaginationGuidDTO.PageSize).ToList();

        foreach (var employeeRequest in employeeRequests)
        { 
            var sender = await _mediator.Send(new GetEmployerQuery(employeeRequest.SenderId));
            //if(sender != null) {
                var employeeResponseDTO = new EmployeeResponseDTO
                {
                    Id = employeeRequest.Id,
                    EmpName = sender != null ?  sender.Name : string.Empty,
                    SenderId = employeeRequest.SenderId,
                    ReceiverId = employeeRequest.ReceiverId,
                    Status = employeeRequest.Status,
                    Description = employeeRequest.Discription,
                    CreatedDateTime = employeeRequest.CreatedAt,
                    LastModifiedDateTime = employeeRequest.UpdatedAt
                };

                responseList.Add(employeeResponseDTO);
            //}
        }

            responseList = responseList.OrderByDescending(r => r.CreatedDateTime).ToList();

            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { items = responseList, request.PaginationGuidDTO.Page, request.PaginationGuidDTO.PageSize, totalCount, hasNextPage = (request.PaginationGuidDTO.Page * request.PaginationGuidDTO.PageSize < totalCount), hasPreviousPage = (request.PaginationGuidDTO.Page > 1) }, message: "Users list");
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new List<StudentDetailsDTO>(), message: ex.Message);
        }
    }
}

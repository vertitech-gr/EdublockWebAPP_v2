using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EmployeeRequest;
using Edu_Block_dev.Modal.DTO;
using MediatR;

public class RejectRequestCommandHandler : IRequestHandler<RejectRequestCommand, EmployeeRequestDTO>
{
    private readonly EduBlockDataContext _dbContext;
    private readonly IMapper _mapper;

    public RejectRequestCommandHandler(EduBlockDataContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<EmployeeRequestDTO> Handle(RejectRequestCommand request, CancellationToken cancellationToken)
    {
        var existingRequest = await _dbContext.Requests
            .FindAsync(request.id);

        if (existingRequest == null)
        {
            return null;
        }

        if (existingRequest.ReceiverId != request.UserId)
        {
            return null;
        }

        if (existingRequest.Status != RequestStatus.Pending)
        {
            return null;
        }
        existingRequest.Status = RequestStatus.Rejected;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var updatedEmployeeRequestDTO = _mapper.Map<EmployeeRequestDTO>(existingRequest);

        return updatedEmployeeRequestDTO;
    }
}

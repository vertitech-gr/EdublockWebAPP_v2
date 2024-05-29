using Edu_Block_dev.Modal.DTO;
using MediatR;
using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EmployeeRequest;
using Edu_Block_dev.CQRS.Query.EduUserRequest;

namespace Edu_Block_dev.CQRS.Command_Handler.EmployeeRequest
{
    public class EmployeeRequestCommandHandler : IRequestHandler<EmployeeRequestCommand, EmployeeRequestResponse>
    {
        private readonly EduBlockDataContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public EmployeeRequestCommandHandler(EduBlockDataContext dbContext, IMapper mapper, IMediator mediator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<EmployeeRequestResponse> Handle(EmployeeRequestCommand request, CancellationToken cancellationToken)
        {
            if (request.RequestDto == null)
            {
                return null;
            }
            Request userRequestEntity = new Request()
            {
                SenderId = request.Employer.UserProfile.Id,
                ReceiverId = request.RequestDto.ReceiverId,
                CreatedBy = request.Employer.UserProfile.Id,
                Discription = request.RequestDto.Discription
            };
            _dbContext.Add(userRequestEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var getRequestQuery = new GetRequestByDetailsQuery(request.Employer.UserProfile.Id, request.RequestDto.ReceiverId);
            var createdDto = await _mediator.Send(getRequestQuery);

            List<Guid> credentialIds = new List<Guid>();

            var createdDtos = new EmployeeRequestResponse() {
                ReceiverId = userRequestEntity.ReceiverId,
                Status = userRequestEntity.Status,
                Discription = userRequestEntity.Discription,
            };
            return createdDtos;
        }
    }
}

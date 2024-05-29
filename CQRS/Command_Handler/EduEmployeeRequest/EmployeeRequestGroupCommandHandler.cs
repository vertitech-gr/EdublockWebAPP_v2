using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EmployeeRequest;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EmployeeRequest
{
    public class EmployeeRequestGroupCommandHandler : IRequestHandler<EmployeeRequestGroupCommand, EmployeeRequestGroupDTO>
    {
        private readonly EduBlockDataContext _dbContext;
        private readonly IMapper _mapper;

        public EmployeeRequestGroupCommandHandler(EduBlockDataContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<EmployeeRequestGroupDTO> Handle(EmployeeRequestGroupCommand request, CancellationToken cancellationToken)
        {
            if (request.RequestGroupDTO == null)
            {
                return null;
            }
            var userRequestEntity = _mapper.Map<EmployeeRequestGroup>(request.RequestGroupDTO);
            _dbContext.Add(userRequestEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            var createdDto = _mapper.Map<EmployeeRequestGroupDTO>(userRequestEntity);
            return createdDto;
        }

    }
}

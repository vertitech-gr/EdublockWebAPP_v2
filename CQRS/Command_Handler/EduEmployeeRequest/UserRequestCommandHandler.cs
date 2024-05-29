using AutoMapper;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduUserRequest;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EmployeeRequest
{
    public class UserRequestCommandHandler : IRequestHandler<CreateUserRequestCommand, ApiResponse<object>>
    {
        private readonly IRepository<UserRequest> _userRequestRepository;
        private readonly IMapper _mapper;

        public UserRequestCommandHandler(IRepository<UserRequest> userRequestRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRequestRepository = userRequestRepository;
        }

        public async Task<ApiResponse<object>> Handle(CreateUserRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                UserRequest userRequestEntity = _mapper.Map<UserRequest>(request.UserRequestDto);
                userRequestEntity.SenderId = request.User.Id;
                await _userRequestRepository.AddAsync(userRequestEntity);
                var createdDto = _mapper.Map<UserRequestDto>(userRequestEntity);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: createdDto, message: "Create request successfull.");

            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: null, message: "Unable to create request.");
            }
        }
    }
}

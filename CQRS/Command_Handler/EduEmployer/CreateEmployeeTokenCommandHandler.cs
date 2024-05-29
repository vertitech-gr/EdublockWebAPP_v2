using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Commands.EduEmployer;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class CreateEmployeeTokenCommandHandler : IRequestHandler<CreateEmployeeTokenCommand, ApiResponse<object>>
    {
        private readonly IRepository<EmployerToken> _employeeTokenRepository;
        private readonly IJwtUtils _jwtUtils;

        public CreateEmployeeTokenCommandHandler(IRepository<EmployerToken> employeeTokenRepository, IJwtUtils jwtUtils)
        {
            _employeeTokenRepository = employeeTokenRepository;
            _jwtUtils = jwtUtils;
        }

        public async Task<ApiResponse<object>> Handle(CreateEmployeeTokenCommand request, CancellationToken cancellationToken)
        {
            try {
                var existingUser = await _employeeTokenRepository.FindAsync(u => u.Name == request.Name);
                if (existingUser != null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.Conflict , message: "Employee token with this email already exists");
                }

                Employer employer = new Employer()
                {
                     Id = request.User.Id,
                };


                var access_token = _jwtUtils.GenerateJwtTokenForEmployee(employer, "api");

                var employerTokenEntity = new EmployerToken
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    UserId = request.User.Id,
                    Token = access_token
                };
                await _employeeTokenRepository.AddAsync(employerTokenEntity);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: employerTokenEntity, message: "Api key generated");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError ,message: ex.Message);

            }
        }
    }
}
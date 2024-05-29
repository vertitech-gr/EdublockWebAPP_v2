using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Commands.EduEmployer;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class DeleteEmployeeTokenCommandHandler : IRequestHandler<DeleteEmployeeTokenCommand, ApiResponse<object>>
    {
        private readonly IRepository<EmployerToken> _employeeTokenRepository;

        public DeleteEmployeeTokenCommandHandler(IRepository<EmployerToken> employeeTokenRepository)
        {
            _employeeTokenRepository = employeeTokenRepository;
        }

        public async Task<ApiResponse<object>> Handle(DeleteEmployeeTokenCommand request, CancellationToken cancellationToken)
        {
            try {
                var existingUser = await _employeeTokenRepository.FindAsync(u => u.Id == request.EmpTokenID);
                await _employeeTokenRepository.DeleteAsync(existingUser.Id );
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: null, message: "Employee token deleted successfully");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError ,message: ex.Message);

            }
        }
    }
}
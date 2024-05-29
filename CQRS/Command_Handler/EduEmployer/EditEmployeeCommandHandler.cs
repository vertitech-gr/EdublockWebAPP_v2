using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Commands.EduEmployer;
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class EditEmployeeCommandHandler : IRequestHandler<EditEmployerCommand, ApiResponse<object>>
    {
        private readonly IRepository<Employer> _employeeRepository;

        public EditEmployeeCommandHandler(IRepository<Employer> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<ApiResponse<object>> Handle(EditEmployerCommand request, CancellationToken cancellationToken)
        {
            try {
                Employer existingEmployee = await _employeeRepository.FindAsync(u => u.Id == request.EditEmployerDTO.guid);
                if (existingEmployee == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.Conflict , message: "Employee don't exists in our Application");
                }

                existingEmployee.Name = request.EditEmployerDTO.Name;
                existingEmployee.Email = request.EditEmployerDTO.Email;
                existingEmployee.Address = request.EditEmployerDTO.Address;
                existingEmployee.Industry = request.EditEmployerDTO.Industry;
                existingEmployee.SpecificIndustry = request.EditEmployerDTO.SpecificIndustry;
                existingEmployee.CountryCode = request.EditEmployerDTO.CountryCode;
                existingEmployee.PhoneNumber = request.EditEmployerDTO.PhoneNumber;
                existingEmployee.Status = request.EditEmployerDTO.Status;

                var addedEmployee = await _employeeRepository.UpdateAsync(existingEmployee.Id, existingEmployee);
               
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, message: "Employee Update successfull");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError ,message: "Employer Updation unsuccessfull");

            }

        }
    }
}

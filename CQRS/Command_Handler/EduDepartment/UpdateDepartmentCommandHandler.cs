using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduDepartment;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduDepartment
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMapper _mapper;

        public UpdateDepartmentCommandHandler(EduBlockDataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<object>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try {
                Department department = await _context.Departments.FindAsync(request.DepartmentRequestDTO.DepartmentID);
                if (department == null)
                {
                    return null;
                }
                department.UpdatedBy = request.DepartmentRequestDTO.UniversityID;
                department.Name = request.DepartmentRequestDTO.Name;
                department.Type = request.DepartmentRequestDTO.Type;


                await _context.SaveChangesAsync(cancellationToken);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: department, message: "Department updated successfully");

            }
            catch (Exception ex) {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, error: ex.Message , message: "Unable to update Department");

            }
        }
    }
}
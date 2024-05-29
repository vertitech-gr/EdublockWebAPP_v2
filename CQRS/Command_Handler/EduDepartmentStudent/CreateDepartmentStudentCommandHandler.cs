using System.Net;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduCertificate;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduCertificate
{
    public class CreateDepartmentStudentCommandHandler : IRequestHandler<CreateDepartmentStudentCommand, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;

        public CreateDepartmentStudentCommandHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<object>> Handle(CreateDepartmentStudentCommand request, CancellationToken cancellationToken)
        {
            try {
                _context.Add(request.DepartmentStudent);
                var result = await _context.SaveChangesAsync(cancellationToken);
                return new ApiResponse<object>(HttpStatusCode.OK, data: result, message: "Department student created successfully");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(HttpStatusCode.InternalServerError, data: null, message: ex.Message);
            }
        }
    }
}
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EmployeeRequest
{
    public class EmployeeRequestQuery : IRequest<ApiResponse<object>>
    {
        public EmployeeResponseDTO employeeResponseDTO { get; }
        public CommanUser CommanUser;
        public PaginationGuidDTO PaginationGuidDTO;

        public EmployeeRequestQuery(PaginationGuidDTO paginationGuidDTO, EmployeeResponseDTO employeeResponse, CommanUser commanUser)
        {
            employeeResponseDTO = employeeResponse;
            CommanUser = commanUser;
            PaginationGuidDTO = paginationGuidDTO;
        }
    }
}
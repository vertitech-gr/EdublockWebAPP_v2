using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduEmployer
{
    public class GetEmployerQuery : IRequest<EmployerDTO>
    {
        public Guid EmployerId { get; }

        public GetEmployerQuery(Guid employerId)
        {
            EmployerId = employerId;
        }
    }
}

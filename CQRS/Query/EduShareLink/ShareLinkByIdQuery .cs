using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduShareLink
{
    public class ShareByUniqueIdQuery : IRequest<ShareRespnseDto>
    {
        public Guid Id { get; set; }

        public ShareByUniqueIdQuery(Guid id)
        {
            Id = id;
        }
    }
}

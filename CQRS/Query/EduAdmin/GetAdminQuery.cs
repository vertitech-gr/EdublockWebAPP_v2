using Edu_Block.DAL.EF;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduAdmin
{
    public class GetAdminQuery
    {
    }

    public class GetAdminByUniqueIdQuery : IRequest<UserWithProfile<Admin>>
    {
        public Guid _uniqueId { get; set; }

        public GetAdminByUniqueIdQuery(Guid uniqueId)
        {
            _uniqueId = uniqueId;
        }
    }
}
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduAdmin
{
    public class GetUniversityUserQuery
    {
    }

    public class GetUniversityUserByUniqueIdQuery : IRequest<UserWithProfile<UniversityUser>>
    {
        public Guid _uniqueId { get; set; }

        public GetUniversityUserByUniqueIdQuery(Guid uniqueId)
        {
            _uniqueId = uniqueId;
        }
    }

    public class GetUniversityByUniversityUserIdQuery : IRequest<UniversityResponseDTO>
    {
        public Guid uniqueId { get; set; }
        public GetUniversityByUniversityUserIdQuery(Guid _uniqueId)
        {
            uniqueId = _uniqueId;
        }
    }

    public class ListUniversityUserQuery : IRequest<ApiResponse<object>>
    {
        public CommanUser User { get; set; }
        public PaginationUniversityUserDTO PaginationUniversityUserDTO { get; set; }

        public ListUniversityUserQuery(CommanUser user, PaginationUniversityUserDTO paginationUniversityUserDTO)
        {
            User = user;
            PaginationUniversityUserDTO = paginationUniversityUserDTO;
        }
    }

    public class NewListUniversityUserQuery : IRequest<ApiResponse<object>>
    {
        public CommanUser User { get; set; }
        public PaginationUniversityUserDTO PaginationUniversityUserDTO { get; set; }

        public NewListUniversityUserQuery(CommanUser user, PaginationUniversityUserDTO paginationUniversityUserDTO)
        {
            User = user;
            PaginationUniversityUserDTO = paginationUniversityUserDTO;
        }
    }

}
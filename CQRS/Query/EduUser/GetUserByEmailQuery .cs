using Edu_Block.DAL.EF;
using EduBlock.Model.DTO;
using MediatR;
using System.Security.Claims;

namespace Edu_Block_dev.CQRS.Query.EduUser
{
    public class GetUserByEmailQuery : IRequest<User>
    {
        public string Email { get; set; }

        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }

    public class GetUserByIdQuery : IRequest<User>
    {
        public Guid Id { get; set; }

        public GetUserByIdQuery(Guid _id)
        {
            Id = _id;
        }
    }

    public class GetNewUsersQuery : IRequest<ApiResponse<object>>
    {
        public PaginationGuidDTO PaginationGuidDTO { get; set; }
        public GetNewUsersQuery(PaginationGuidDTO paginationGuidDTO)
        {
            PaginationGuidDTO = paginationGuidDTO;
        }
    }

    public class GetUserCertificatesQuery : IRequest<List<Certificate>>
    {
        public Guid UserProfileID { get; set; }
        public GetUserCertificatesQuery(Guid userProfileID)
        {
            UserProfileID = userProfileID;
        }
    }

    public class GetUsersQuery : IRequest<ApiResponse<object>>
    {
        public PaginationUniversityUserDTO PaginationUniversityUserDTO { get; set; }
        public GetUsersQuery(PaginationUniversityUserDTO paginationUniversityUserDTO)
        {
            PaginationUniversityUserDTO = paginationUniversityUserDTO;
        }
    }

    public class GetUniversityUsersQuery : IRequest<ApiResponse<object>>
    {
        public PaginationUniversityUserDTO PaginationUniversityUserDTO { get; set; }
        public GetUniversityUsersQuery(PaginationUniversityUserDTO paginationUniversityUserDTO)
        {
            PaginationUniversityUserDTO = paginationUniversityUserDTO;
        }
    }


    public class UpdateUserLoginStatus : IRequest<ApiResponse<object>>
    {
        public bool type { get; set; }
        public string email { get; set; }
        public UpdateUserLoginStatus(bool _type, string _email)
        {
            type = _type;
            email = _email;
        }
    }

    public class GetUserByUniqueIdQuery : IRequest<UserWithProfile<User>>
    {
        public Guid _uniqueId { get; set; }

        public GetUserByUniqueIdQuery(Guid uniqueId)
        {
            _uniqueId = uniqueId;
        }
    }



    public class GetUserDetailsQuery
    {
        public ClaimsPrincipal User { get; }

        public GetUserDetailsQuery(ClaimsPrincipal user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
        }
    }
}

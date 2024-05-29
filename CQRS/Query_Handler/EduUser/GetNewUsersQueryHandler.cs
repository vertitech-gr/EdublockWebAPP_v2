using Edu_Block.DAL.EF;
using MediatR;
using Edu_Block_dev.CQRS.Query.EduUser;
using EduBlock.Model.DTO;
using System.Linq.Expressions;
using Edu_Block.DAL;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetNewUsersQueryHandler : IRequestHandler<GetNewUsersQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<DockIoDID> _dockIoDIDRepository;

        public GetNewUsersQueryHandler(EduBlockDataContext context, IRepository<UserProfile> userProfileRepository, IRepository<DockIoDID> dockIoDIDRepository)
        {
            _context = context;
            _userProfileRepository = userProfileRepository;
            _dockIoDIDRepository = dockIoDIDRepository;
        }

        public async Task<ApiResponse<object>> Handle(GetNewUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = _context.User.Where( u => u.IsDeleted == false ).ToList();

                if (request.PaginationGuidDTO.SearchTerm != string.Empty)
                {
                     users = users.Where(p =>
                                        p.Name.ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
                                        p.Email.ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower())).ToList();
                }

                var toatlCount = users.Count();

                var query = users.Skip((request.PaginationGuidDTO.Page - 1) * request.PaginationGuidDTO.PageSize).Take(request.PaginationGuidDTO.PageSize).ToList();
                IQueryable<StudentDetailsDTO> StudentDetailsDTOList;
                List<StudentDetailsDTO> StudentDetailsDTOs = new List<StudentDetailsDTO>();
                foreach ( var user in query )
                {
                    StudentDetailsDTO StudentDetailsDTO = new StudentDetailsDTO();
                    StudentDetailsDTO.User = user;
                    StudentDetailsDTO.UserProfile = user != null ? await _userProfileRepository.FindAsync(up => up.UserID == user.Id) : null;
                    StudentDetailsDTO.Dock = StudentDetailsDTO.UserProfile != null ? await _dockIoDIDRepository.FindAsync(d => d.UserProfileId == StudentDetailsDTO.UserProfile.Id) : null;
                    StudentDetailsDTOs.Add(StudentDetailsDTO);
                }

                StudentDetailsDTOList = StudentDetailsDTOs.AsQueryable();

                var toas = StudentDetailsDTOs.Count();

                if (request.PaginationGuidDTO.guid != Guid.Empty)
                {
                    StudentDetailsDTOList = StudentDetailsDTOList.Where(up => up.User.Id == request.PaginationGuidDTO.guid);
                }

                //if (!string.IsNullOrWhiteSpace(request.PaginationGuidDTO.SearchTerm))
                //{
                //    StudentDetailsDTOList = StudentDetailsDTOList.Where(p =>
                //     p.User.Name.ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()) ||
                //     p.User.Email.ToLower().Contains(request.PaginationGuidDTO.SearchTerm.ToLower()));
                //}

                if (request.PaginationGuidDTO.SortOrder?.ToLower() == "desc")
                {
                    StudentDetailsDTOList = StudentDetailsDTOList.OrderByDescending(GetSortProperty(request));
                }
                else
                {
                    StudentDetailsDTOList = StudentDetailsDTOList.OrderBy(GetSortProperty(request));
                }

                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { items = StudentDetailsDTOList, request.PaginationGuidDTO.Page, request.PaginationGuidDTO.PageSize, toatlCount, hasNextPage = (request.PaginationGuidDTO.Page * request.PaginationGuidDTO.PageSize < toatlCount), hasPreviousPage = (request.PaginationGuidDTO.Page > 1) }, message: "Users list");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new List<StudentDetailsDTO>(), message: ex.Message);
            }
        }

        private Expression<Func<StudentDetailsDTO, object>> GetSortProperty(GetNewUsersQuery request)
        {
            switch (request.PaginationGuidDTO.SortColumn?.ToLower())
            {
                case "name":
                    return u => u.User.Name;
                case "email":
                    return u => u.User.Email;
                default:
                    return u => u.User.Id;
            }
        }

    }
}
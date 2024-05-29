using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduUserRequest;
using Edu_Block_dev.Helpers;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUserRequest
{
    public class OutgoingRequestQueryHandler : IRequestHandler<GetOutgoingRequestsQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public OutgoingRequestQueryHandler(EduBlockDataContext context, IMapper mapper, IMediator mediator)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponse<object>> Handle(GetOutgoingRequestsQuery request, CancellationToken cancellationToken)
        {
            try {

                IQueryable<RequestResponseDto> StudentRequests = _context.UserRequest
               .Join(_context.User, r => r.SenderId, u => u.Id, (r, u) => new
               {
                   UserRequests = r,
                   User = u
               })
               .Join(_context.DepartmentStudents, r => r.User.Id, ds => ds.StudentId, (r, ds) => new RequestResponseDto
               {
                   Id = r.UserRequests.Id,
                   SenderId = r.UserRequests.SenderId,
                   ReceiverId = r.UserRequests.ReceiverId,
                   Department = r.UserRequests.RequestType,
                   EndDate = ds.EndDate,
                   GraduationYear = r.UserRequests.GraduationYear,
                   lastUpdateDate = r.UserRequests.UpdatedAt,
                   Remark = r.UserRequests.Remark,
                   StartDate = ds.StartDate,
                   Status = r.UserRequests.Status,
                   Email = r.User.Email,
                   Name = r.User.Name
               });

                if (request.PaginationGuidDTO.guid != Guid.Empty)
                {
                    StudentRequests = StudentRequests.Where(srs => srs.SenderId == request.PaginationGuidDTO.guid);
                }

                if (!string.IsNullOrWhiteSpace(request.PaginationGuidDTO.SearchTerm))
                {
                    StudentRequests = StudentRequests.Where(p =>
                        p.Name.ToLower().Contains(request.PaginationGuidDTO.SearchTerm) ||
                        ((string)p.Email).ToLower().Contains(request.PaginationGuidDTO.SearchTerm));
                }
                if (request.PaginationGuidDTO.SortOrder?.ToLower() == "desc")
                {
                    StudentRequests = StudentRequests.OrderByDescending(GetSortProperty(request));
                }
                else
                {
                    StudentRequests = StudentRequests.OrderBy(GetSortProperty(request));

                }

                var StudentRequestList= await PagedList<RequestResponseDto>.CreateAsync(
                                StudentRequests,
                                request.PaginationGuidDTO.Page,
                request.PaginationGuidDTO.PageSize);



                return new ApiResponse<object>(HttpStatusCode.OK, data: StudentRequestList, message: "Student requests list");

            }
            catch (Exception ex) {
                return new ApiResponse<object>(HttpStatusCode.Unauthorized, data: null, message: "Student requests list");
            }
        }

        public static Expression<Func<RequestResponseDto, object>> GetSortProperty(GetOutgoingRequestsQuery request)
        {
            return request.PaginationGuidDTO.SortColumn?.ToLower() switch
            {
                "name" => employer => employer.Name,
                "email" => employer => employer.Email,
                "lastUpdateDate" => employer => employer.lastUpdateDate,
                _ => employer => employer.lastUpdateDate
            };
        }
    }
}

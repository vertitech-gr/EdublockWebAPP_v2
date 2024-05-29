using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduUserRequest;
using Edu_Block_dev.Helpers;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUserRequest
{
    public class GetRequestsQueryHandler : IRequestHandler<GetRequestsQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetRequestsQueryHandler(EduBlockDataContext context, IMapper mapper, IMediator mediator)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponse<object>> Handle(GetRequestsQuery request, CancellationToken cancellationToken)
        {
            try {

               IQueryable<RequestResponseDto> StudentRequests = _context.UserRequest
               .Join(_context.User, r => r.SenderId, u => u.Id, (r, u) => new
                {
                   UserRequests = r,
                   User = u
                })
               .Join(_context.DepartmentStudents, r => r.User.Id, ds => ds.StudentId, (r, ds) => new 
                {
                   UserRequests = r.UserRequests,
                   User = r.User,
                   DepartmentStudent = ds
               }).Join(_context.Departments, r => r.DepartmentStudent.DepartmentId , d => d.Id, (r, d) => new RequestResponseDto
                {
                    Id = r.UserRequests.Id,
                    SenderId = r.UserRequests.SenderId,
                    ReceiverId = r.UserRequests.ReceiverId,
                    UniversityId = r.DepartmentStudent.UniversityId,
                    DepartmentId = d.Id,
                    Department = r.UserRequests.RequestType,
                    EndDate = r.DepartmentStudent.EndDate,
                    GraduationYear = r.UserRequests.GraduationYear,
                    lastUpdateDate = r.UserRequests.UpdatedAt,
                    Remark = r.UserRequests.Remark,
                    StartDate = r.DepartmentStudent.StartDate,
                    Status = r.UserRequests.Status,
                    Email = r.User.Email,
                    Name = r.User.Name,
                    DepartmentOutput =  d
               });

                if( request.PaginationReceivedRequestDTO.Status != MessageStatus.Blank)
                {
                    StudentRequests = StudentRequests.Where(srs => srs.Status == request.PaginationReceivedRequestDTO.Status);
                }

                if (request.PaginationReceivedRequestDTO.Guid != Guid.Empty)
                {
                    StudentRequests = StudentRequests.Where(srs => srs.ReceiverId == request.PaginationReceivedRequestDTO.Guid);
                }

                if (request.PaginationReceivedRequestDTO.SenderId != null && request.PaginationReceivedRequestDTO.SenderId != Guid.Empty)
                {
                    StudentRequests = StudentRequests.Where(srs => srs.SenderId == request.PaginationReceivedRequestDTO.SenderId);
                }

                if (request.PaginationReceivedRequestDTO.DepartmentId != Guid.Empty)
                {
                    StudentRequests = StudentRequests.Where(srs => srs.DepartmentId == request.PaginationReceivedRequestDTO.DepartmentId);
                }

                if (request.PaginationReceivedRequestDTO.UniversityId != Guid.Empty)
                {
                    StudentRequests = StudentRequests.Where(srs => srs.UniversityId == request.PaginationReceivedRequestDTO.UniversityId);
                }

                if (!string.IsNullOrWhiteSpace(request.PaginationReceivedRequestDTO.SearchTerm))
                {
                    StudentRequests = StudentRequests.Where(p =>
                        p.Name.ToLower().Contains(request.PaginationReceivedRequestDTO.SearchTerm.ToLower()) ||
                        ((string)p.Email).ToLower().Contains(request.PaginationReceivedRequestDTO.SearchTerm.ToLower()) ||
                        p.Department.ToLower().Contains(request.PaginationReceivedRequestDTO.SearchTerm.ToLower()) 
                        );
                }

                if (request.PaginationReceivedRequestDTO.SortOrder?.ToLower() == "desc")
                {
                    StudentRequests = StudentRequests.OrderByDescending(GetSortProperty(request));
                }
                else
                {
                    StudentRequests = StudentRequests.OrderBy(GetSortProperty(request));

                }

                StudentRequests = StudentRequests.Where( sr => sr.DepartmentOutput.IsDeleted == false).AsQueryable();


                var StudentRequestList= await PagedList<RequestResponseDto>.CreateAsync(
                                StudentRequests,
                                request.PaginationReceivedRequestDTO.Page,
                request.PaginationReceivedRequestDTO.PageSize);

                var statusCounts = await StudentRequests
                    .GroupBy(r => r.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var statusCountsDict = statusCounts.ToDictionary(x => x.Status, x => x.Count);

                var response = new
                {
                    items = StudentRequestList,
                    page = request.PaginationReceivedRequestDTO.Page,
                    pageSize = request.PaginationReceivedRequestDTO.PageSize,
                    totalCount = StudentRequestList.TotalCount,
                    hasNextPage = StudentRequestList.HasNextPage,
                    hasPreviousPage = StudentRequestList.HasPreviousPage,
                    statusCounts = statusCountsDict
                };

                return new ApiResponse<object>(HttpStatusCode.OK, data: response, message: "Student requests list");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(HttpStatusCode.Unauthorized, data: null, message: "Student requests list");
            }
        }

        public static Expression<Func<RequestResponseDto, object>> GetSortProperty(GetRequestsQuery request)
        {
            return request.PaginationReceivedRequestDTO.SortColumn?.ToLower() switch
            {
                "name" => employer => employer.Name,
                "email" => employer => employer.Email,
                "status" => employer => employer.Status,
                "lastUpdateDate" => employer => employer.lastUpdateDate,
                _ => employer => employer.lastUpdateDate
            };
        }
    }
}

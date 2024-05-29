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
    public class GetRequestMessageQueryHandler : IRequestHandler<GetRequestMessageQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetRequestMessageQueryHandler(EduBlockDataContext context, IMapper mapper, IMediator mediator)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponse<object>> Handle(GetRequestMessageQuery request, CancellationToken cancellationToken)
        {
            try {

                IQueryable<RequestMessageResponseDTO> RequestMessages = _context.RequestMessages
                    .Join(_context.UserRequest, ur => ur.RequestId, r => r.Id, (ur , r) => new RequestMessageResponseDTO
                    {
                         UserRequest = r,
                         RequestMessages = ur
                    })
                    .ToList().AsQueryable();

                foreach(RequestMessageResponseDTO requestMessageResponseDTO in RequestMessages)
                {
                    if(requestMessageResponseDTO.RequestMessages.Type == Authorization.Role.STUDENT)
                    {
                        User user = _context.User.Where(u => u.Id == requestMessageResponseDTO.RequestMessages.SenderId).FirstOrDefault();
                        requestMessageResponseDTO.CommanUser = _mapper.Map<CommanUser>(user);
                    }
                    else
                    {
                        University university = _context.Universities.Where(u => u.Id == requestMessageResponseDTO.RequestMessages.SenderId).FirstOrDefault();
                        requestMessageResponseDTO.CommanUser = _mapper.Map<CommanUser>(university);
                    }
                }
              

                if (request.TypedPaginationGuidDTO.guid != Guid.Empty)
                {
                    RequestMessages = RequestMessages.Where(srs => srs.RequestMessages.RequestId == request.TypedPaginationGuidDTO.guid);
                }

                if (!string.IsNullOrWhiteSpace(request.TypedPaginationGuidDTO.SearchTerm))
                {
                    RequestMessages = RequestMessages.Where(p =>
                        p.RequestMessages.Description.ToLower().Contains(request.TypedPaginationGuidDTO.SearchTerm));
                }
                if (request.TypedPaginationGuidDTO.SortOrder?.ToLower() == "desc")
                {
                    RequestMessages = RequestMessages.OrderByDescending(GetSortProperty(request));
                }
                else
                {
                    RequestMessages = RequestMessages.OrderBy(GetSortProperty(request));
                }

                var RequestMessageList = await PagedList<RequestMessageResponseDTO>.CreateAsync(
                                RequestMessages,
                                request.TypedPaginationGuidDTO.Page,
                request.TypedPaginationGuidDTO.PageSize);

                return new ApiResponse<object>(HttpStatusCode.OK, data: RequestMessages, message: "Request Message list");

            }
            catch (Exception ex) {
                return new ApiResponse<object>(HttpStatusCode.Unauthorized, data: null, message: "Request Message list");
            }
        }

        public static Expression<Func<RequestMessageResponseDTO, object>> GetSortProperty(GetRequestMessageQuery request)
        {
            return request.TypedPaginationGuidDTO.SortColumn?.ToLower() switch
            {
                "Description" => rm => rm.RequestMessages.Description,
                "CreatedAt" => rm => rm.RequestMessages.CreatedAt,
                _ => rm => rm.RequestMessages.CreatedAt
            };
        }
    }
}

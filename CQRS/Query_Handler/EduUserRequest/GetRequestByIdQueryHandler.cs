using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduUserRequest;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUserRequest
{
    public class GetRequestByIdQueryHandler : IRequestHandler<GetRequestByDetailsQuery, Request>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<Request> _request;

        public GetRequestByIdQueryHandler(EduBlockDataContext context, IRepository<Request> repository)
        {
            _context = context;
            _request = repository;
        }
        public async Task<Request> Handle(GetRequestByDetailsQuery query, CancellationToken cancellationToken)
        {
            return await _request.FindAsync(u => u.SenderId == query.SenderId || u.ReceiverId == query.ReceiverId);
        }
    }
}

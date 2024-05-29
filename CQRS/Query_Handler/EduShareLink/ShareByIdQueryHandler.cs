using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduShareLink;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduShareLink
{
    public class ShareByIdQueryHandler : IRequestHandler<ShareByUniqueIdQuery, ShareRespnseDto>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMapper _mapper;

        public ShareByIdQueryHandler(EduBlockDataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ShareRespnseDto> Handle(ShareByUniqueIdQuery request, CancellationToken cancellationToken)
        {
            var share = await _context.Shares
                .SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            var shareDTO = _mapper.Map<ShareRespnseDto>(share);
            return shareDTO;
        }
    }

}

using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduShareCredentialQuery;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduShareCredential
{
    public class ShareCredentialQueryHandler : IRequestHandler<ShareCredentialQuery, ShareCredentialDTO>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMapper _mapper;

        public ShareCredentialQueryHandler(EduBlockDataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ShareCredentialDTO> Handle(ShareCredentialQuery request, CancellationToken cancellationToken)
        {
            var shareCredential = await _context.ShareCredentials
                .SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            var shareDTO = _mapper.Map<ShareCredentialDTO>(shareCredential);
            return shareDTO;
        }
    }

}

using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduShareCredential
{
    public class ShareCredentialCommand : IRequest<ShareCredential>
    {
        public ShareCredentialDTO shareCredentialDTO {  get; set; }

        public ShareCredentialCommand(ShareCredentialDTO _shareCredentialDTO)
        {
            this.shareCredentialDTO = _shareCredentialDTO;
        }
    }
}

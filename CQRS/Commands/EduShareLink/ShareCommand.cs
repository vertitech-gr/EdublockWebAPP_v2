using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Block_dev.CQRS.Commands.EduShareLink
{
    public class ShareCommand : IRequest<IActionResult>
    {
        public ShareDTO shareDTO {  get; set; }
        public CommanUser User { get; set; }

        public ShareCommand(ShareDTO _shareDTO_, CommanUser _user)
        {
            shareDTO = _shareDTO_;
            User = _user;
        }
    }

    public class ShareVerificationCommand : IRequest<ShareVerificationResponse>
    {
        public ShareVerificationDTO VerificationDTO { get; set; }
        public CommanUser User { get; set; }
        public ShareVerificationCommand(ShareVerificationDTO verificationDTO, CommanUser user)
        {
            VerificationDTO = verificationDTO;
            User = user;
        }
    }


}

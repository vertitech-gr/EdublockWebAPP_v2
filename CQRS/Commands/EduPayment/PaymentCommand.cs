using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduShareCredential
{
    public class PaymentUrlCommand : IRequest<ApiResponse<object>>
    {
        public CommanUser? User;
        public PaymentUrlDTO PaymentUrlDTO; 
        public PaymentUrlCommand(CommanUser user, PaymentUrlDTO paymentUrlDTO)
        {
            User = user;
            PaymentUrlDTO = paymentUrlDTO;
        }
    }

    public class PaymentAccountCommand : IRequest<ApiResponse<object>>
    {
        public CommanUser? User;
        public PaymentAccountCommand(CommanUser? user)
        {
            User = user;
        }
    }
}
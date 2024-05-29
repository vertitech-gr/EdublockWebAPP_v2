using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduUserRequest
{
    public class PaymentDetailsCommand : IRequest<ApiResponse<object>>
    {
        public PaymentDetail PaymentDetail;
        public PaymentDetailsCommand(PaymentDetail paymentDetail)
        {
            PaymentDetail = paymentDetail;
        }
    }
}
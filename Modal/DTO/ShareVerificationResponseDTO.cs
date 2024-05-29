using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
    public enum VerificationStatus
    {
        Valid = 1,
        Unauthorized = 2,
        Expiry = 3,
        Invalid = 4,
        IncompletePayment = 5,
        subscription = 6,
        balance = 7
    }

    public class ShareVerificationResponse
    {
        public VerificationStatus Status { get; set; }
        public string Message { get; set; }
        public Share Share { get; set; }
        public decimal Amount { get; set; }
    }


}

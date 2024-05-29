using System.Numerics;

namespace Edu_Block_dev.Modal.DTO
{
    public class PaymentDetialsResponseDTO
    {
        public string PlanName { get; set; }
        public string Credits { get; set; }
        public string Coins { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string TransactionID { get; set; }
        public string Mode { get; set; }
        public string Email { get; set; }
    }
}
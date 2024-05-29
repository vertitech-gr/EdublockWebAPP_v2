using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
    public class BalanceDTO
    {
        public decimal Amount { get; set; }
        public AvailableSubscription AvailableSubscription { get; set; }
        public PurchaseSubscription PurchaseSubscription { get; set; }
    }
}

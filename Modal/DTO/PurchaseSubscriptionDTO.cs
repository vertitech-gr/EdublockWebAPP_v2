using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
    public class PurchaseSubscriptionDTO
    {
        public Guid AvailableSubscriptionID { get; set; } 
    }

    public class PurchaseCoinsDTO
    {
        public string token { get; set; }
    }

    public class PurchaseSubscriptionResponseDTO: PurchaseSubscription
    {
        public string Name { get; set; }
    }
}
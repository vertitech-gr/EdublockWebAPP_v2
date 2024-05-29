namespace Edu_Block_dev.Modal.DTO
{

    public class AvailableSubscriptionDto
    {
        public SubscriptionType Type { get; set; }
        public int Coins { get; set; }
        public int Amount { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
    }

    public enum SubscriptionType
    {
        Recurring = 2,
        OneTime = 1
    }

}

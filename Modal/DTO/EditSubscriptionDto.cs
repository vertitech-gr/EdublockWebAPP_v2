namespace Edu_Block_dev.Modal.DTO
{

    public class EditSubscriptionDto
    {
        public Guid Id { get; set; }
        public SubscriptionType Type { get; set; }
        public int Coins { get; set; }
        public int Amount { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
    }
}
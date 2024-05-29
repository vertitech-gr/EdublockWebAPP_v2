using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.Modal.DTO
{
    public class PaymentUrlDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        //public string Amount { get; set; }
        public string Description { get; set; }
        public Guid AvailableSubscriptionID { get; set; }

    }
}
using Newtonsoft.Json;

namespace Edu_Block_dev.Modal.DTO
{
    public class PaymentTransactionDto
    {
        public string Account { get; set; }
        public string Action { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}
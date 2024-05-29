using Newtonsoft.Json;

namespace Edu_Block_dev.Modal.DTO
{
    public class PaymentOutputDTO
    {
        public string Account { get; set; }
        public string Action { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        [JsonProperty("alt_key")]
        public string AltKey { get; set; }
        public string transaction { get; set; }
        public DateTime created { get; set; }
    }
}
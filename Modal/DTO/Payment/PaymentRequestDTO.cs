using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Edu_Block_dev.Modal.DTO
{
    public class PaymentRequest
    {
        [JsonProperty("payment")]
        public Payment Payment { get; set; }
    }

    public class Payment
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("success_url")]
        public string SuccessUrl { get; set; }
        [JsonProperty("cancel_url")]
        public string CancelUrl { get; set; }
        [JsonProperty("alt_key")]
        public string AltKey { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("account")]
        public Account Account { get; set; }
    }

    public class AccountDTO
    {
        public Account Account { get; set; }
    }

    public class Account
    {
        [JsonProperty("crmkey")]
        public string CrmKey { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
    }

    public class PaymentResponse
    {
        public string Url { get; set; }
    }

    public class AccountRequest
    {
        public Account Account { get; set; }
    }

    public class AccountResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Crm { get; set; }
        public string CrmKey { get; set; }
        public string Status { get; set; }
        public bool DisconnectedAccount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullAddress { get; set; }
    }
}
using Newtonsoft.Json;

namespace Edu_Block_dev.Modal.Dock
{
	public class DockCredentialDTO
    {
        [JsonProperty("schema")]
        public string Schema { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public List<string> Type { get; set; }
        [JsonProperty("issuer")]
        public string Issuer { get; set; }
        [JsonProperty("issuanceDate")]
        public string IssuanceDate { get; set; }
        [JsonProperty("subject")]
        public object Subject { get; set; }
        [JsonProperty("status")]
        public object Status { get; set; }
    }
}


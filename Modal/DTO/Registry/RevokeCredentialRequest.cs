using Newtonsoft.Json;

namespace Edu_Block_dev.Modal.DTO
{
    public class RevokeCredentialRequest
    {
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("credentialIds")]
        public List<string> CredentialIds { get; set; }
    }
}
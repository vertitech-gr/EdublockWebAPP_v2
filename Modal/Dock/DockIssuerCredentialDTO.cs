using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Newtonsoft.Json;

namespace Edu_Block_dev.Modal.Dock
{
	public class DockIssuerCredentialDTO
	{
        [JsonProperty("anchor")]
        public bool Anchor { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("persist")]
        public bool Persist { get; set; }
        [JsonProperty("template")]
        public string Template { get; set; }
        [JsonProperty("credential")]
        public DockCredentialDTO Credential { get; set; }
        [JsonProperty("recipientEmail")]
        public string RecipientEmail { get; set; }
        [JsonProperty("emailMessage")]
        public string EmailMessage { get; set; }
        [JsonProperty("distribute")]
        public bool Distribute { get; set; }
    }
}
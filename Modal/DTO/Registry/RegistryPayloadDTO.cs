using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Edu_Block_dev.Modal.DTO
{
    public class RegistryPayloadDTO
    {
        [JsonProperty("addOnly")]
        public bool AddOnly { get; set; }

        [JsonProperty("policy")]
        public List<string> Policy { get; set; }

        [JsonProperty("registryType")]
        public string RegistryType { get; set; }
    }
}
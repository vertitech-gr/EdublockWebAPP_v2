using Newtonsoft.Json;

namespace Edu_Block_dev.Modal.DTO
{

    public class PolicyDTO
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("policy")]
        public List<string> Policy { get; set; }

        [JsonProperty("addOnly")]
        public bool AddOnly { get; set; }
    }

    public class DataDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("policy")]
        public PolicyDTO Policy { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class RegistryResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("data")]
        public DataDTO Data { get; set; }
    }

}

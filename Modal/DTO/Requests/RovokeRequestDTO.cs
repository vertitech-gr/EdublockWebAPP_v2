using Newtonsoft.Json;

namespace Edu_Block_dev.Modal.DTO
{
    public class RevokeData
    {
        [JsonProperty("revokeIds")]
        public List<int> RevokeIds { get; set; }
    }

    public class RevokeRequestDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("data")]
        public RevokeData Data { get; set; }
    }
}
namespace Edu_Block_dev.Modal.DTO
{
    public class WebhookPayloadDTO
    {
        public string Key { get; set; }
        public string Application { get; set; }
        public string InstanceKey { get; set; }
        public string Client { get; set; }
        public string Action { get; set; }
        public string Payment { get; set; }
        public string Transaction { get; set; }
        public string Contract { get; set; }
        public DateTime Created { get; set; }
    }
}
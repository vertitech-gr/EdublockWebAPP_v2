using Edu_Block_dev.Modal.Base;

namespace Edu_Block_dev.Modal.Dock
{
    public class CreateHandlerResponse : BaseCommandResponse
    {
        public CreateHandlerResponse(bool success = true, string message = null) : base(success, message){}
        public string Did { get; set; }
        public string Controller { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
    }
}
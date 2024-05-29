using Edu_Block_dev.Modal.Base;

namespace Edu_Block_dev.Modal.Dock
{
    public class IssuerProfileResponse : BaseCommandResponse
    {
        public IssuerProfileResponse(bool success = true, string message = null) : base(success, message) { }
    }
}
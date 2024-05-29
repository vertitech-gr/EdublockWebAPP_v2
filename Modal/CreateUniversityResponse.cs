using Edu_Block_dev.Modal.Base;

namespace Edu_Block_dev.Modal
{
    public class CreateUniversityResponse : BaseCommandResponse
    {
     public CreateUniversityResponse(bool success = true, string message = null) : base(success, message) { }

    }
}

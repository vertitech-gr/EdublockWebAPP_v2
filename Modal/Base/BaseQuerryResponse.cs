namespace Edu_Block_dev.Modal.Base
{
    public class BaseQuerryResponse
    {
        public bool Success { get; set; } = true;
        public BaseQuerryResponse(bool success = true)
        {
            Success = success;
        }
    }
}


namespace Edu_Block_dev.Modal.Base
{
	public class BaseCommandResponse
	{
        public bool Success { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
        public BaseCommandResponse(bool success = true, string errorMessage = null)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
}


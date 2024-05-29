namespace EduBlock.Model.DTO;

public class BaseQuerryResponse
{
    public bool Success { get; set; } = true;

    //To be called in cases of errors, by default we assume success
    public BaseQuerryResponse(bool success = true)
    {
        Success = success;
    }   
}


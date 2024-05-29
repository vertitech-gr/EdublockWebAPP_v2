namespace Edu_Block_dev.Helpers;

public class Jwt
{
    public string? Secret { get; set; }
    public string? Audience { get; set; }
    public string? Issuer { get; set; }
}
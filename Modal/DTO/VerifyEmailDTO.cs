using System;
namespace EduBlock.Model.DTO
{
	public class VerifyEmailDTO
    {
        public string Key { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}


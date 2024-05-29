using System;
using System.ComponentModel.DataAnnotations;

namespace EduBlock.Model.DTO
{
	public class VerifyEmailGuidDTO
    {
        public string Key { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
    }
    public class EmailRoleGuidDTO
    {
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
    }
}
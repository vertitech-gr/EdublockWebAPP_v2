using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edu_Block.DAL.EF
{
    public enum OtpType
    {
        REGISTER,
        FORGET_PASSWORD
    }

    public class Otp
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("UserProfile")]
        public Guid UserProfileId { get; set; }

        [Required]
        public string OtpCode { get; set; }

        [Required]
        public string key { get; set; }

        [Required]
        public DateTime ExpiryTime { get; set; }

        public bool IsVerified { get; set; } = false;

        [Required]
        public OtpType OtpType { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual UserProfile UserProfile { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Edu_Block.DAL.EF
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string RollNo { get; set; }

        [Required]
        [JsonIgnore]
        public string Password { get; set; }

        public bool Status { get; set; }

        public bool loginStatus { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public virtual ICollection<Otp>? Otps { get; set; }

        public static implicit operator User(University v)
        {
            throw new NotImplementedException();
        }
    }

}


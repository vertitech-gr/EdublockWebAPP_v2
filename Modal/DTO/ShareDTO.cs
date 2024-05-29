using Edu_Block_dev.DAL.EF;
using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.Modal.DTO
{
    public class ShareDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string EmailId { get; set; }

        [Required]
        [EnumDataType(typeof(ShareType), ErrorMessage = "Invalid ShareType value. Allowed values are 1 or 2.")]
        public ShareType Type { get; set; }

        [Required]
        public Guid ResourceId { get; set; }

        public Guid RequestId { get; set; }

        public bool QR { get; set; } = false;

        public List<Guid>? Credentials { get; set; }

    }

    public class ShareRespnseDto
    {
        public Guid Id { get; set; }

        public string ReceiverId { get; set; }

        [EnumDataType(typeof(ShareType), ErrorMessage = "Invalid ShareType value. Allowed values are 1 or 2.")]
        public ShareType Type { get; set; }
        public Guid ResourceId { get; set; }

    }

}

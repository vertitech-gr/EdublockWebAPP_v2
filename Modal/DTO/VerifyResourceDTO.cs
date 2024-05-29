using Edu_Block_dev.DAL.EF;
using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.Modal.DTO
{
    public class VerifyResourceDTO
    {
        [Required]
        [EnumDataType(typeof(ShareType), ErrorMessage = "Invalid ShareType value. Allowed values are 1 or 2.")]
        public ShareType Type { get; set; }

        [Required]
        public string Token { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;

namespace Edu_Block_dev.Modal.DTO
{
    public class UniversityRequestDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        public string? Description { get; set; }

        public Guid? RoleGuid { get; set; }

        public Guid? UniversityId { get; set; }

        [Required(ErrorMessage = "CountryCode is required")]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }

        public string Type { get; set; }

        //[ValidateImage(5242880, ErrorMessage = "Invalid image format or size for Logo")]
        public IFormFile? Logo { get; set; }
    }

    public class ValidateImageAttribute : ValidationAttribute
    {
        private readonly int _maxSizeInBytes;

        public ValidateImageAttribute(int maxSizeInBytes)
        {
            _maxSizeInBytes = maxSizeInBytes;
        }

        public override bool IsValid(object value)
        {
            if (value is IFormFile file)
            {

                var xx = file.OpenReadStream();
                var att = Image.FromStream(file.OpenReadStream());

                try
                {
                    using (var image = Image.FromStream(file.OpenReadStream()))
                    {
                        bool isValidFormat = image.RawFormat.Equals(ImageFormat.Jpeg) ||
                                              image.RawFormat.Equals(ImageFormat.Png) ||
                                              image.RawFormat.Equals(ImageFormat.Gif) ||
                                              image.RawFormat.Equals(ImageFormat.Bmp);

                        bool isValidSize = file.Length <= _maxSizeInBytes;

                        return isValidFormat && isValidSize;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}

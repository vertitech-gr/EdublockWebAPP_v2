namespace Edu_Block_dev.Modal.DTO
{
    public class EmployerDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Industry { get; set; }
        public string SpecificIndustry { get; set; }
        public string Password { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class EditEmployerDTO
    {
        public Guid guid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Industry { get; set; }
        public string SpecificIndustry { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public bool Status { get; set; }
    }
}

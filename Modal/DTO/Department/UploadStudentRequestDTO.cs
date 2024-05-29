namespace Edu_Block_dev.Modal.DTO
{
    public class UploadStudentRequestDTO
    {
        public IFormFile File { get; set; }
        public Guid UniversityId { get; set; }
        public string Issuer { get; set; }
        public Guid CertificateTemplateId { get; set; }
        public string DegreeName { get; set; }
        public string Schema { get; set; }
        public string MappedArray { get; set; }
    }
}
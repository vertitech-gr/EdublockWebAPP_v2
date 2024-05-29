namespace Edu_Block_dev.Modal.DTO
{
  public class CertificateResponseDTO
  {
        public Guid CertificateId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DegreeAwardedDate { get; set; }
        public string MarksAndGrades { get; set; }
        public string CertificationType { get; set; }
        public string PricipalName { get; set; }
        public string OfficialId { get; set; }
        public string certifcateText { get; set; }
        public Guid UniversityId { get; set; }
        public Guid DepartmentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace Edu_Block_dev.Modal.DTO
{
  public class UserRequestDto
  {
        public Guid ReceiverId { get; set; }
        public string Remark { get; set; }
        public MessageStatus Status { get; set; }
        public string RequestType { get; set; }
        public string GraduationYear { get; set; }
  }

  public class CertificateDetail
  {
        public Guid id { get; set; }
        public string Name { get; set; }
  }

  public class CertificateView
  {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string degreeType { get; set; }
        public string degreeName { get; set; }
        public string path { get; set; }
        public string fileName { get; set; }
        public bool status { get; set; }
        public DateTime degreeAwardedDate { get; set; }
        public DateTime dateOfBirth { get; set; }
        public DateTime issuanceDate { get; set; }
        public DateTime expireDate { get; set; }
        public int certificateTemplateID { get; set; }
        public Guid userProfileID { get; set; }
        [JsonIgnore]
        public string credentialsJson { get; set; }
  }
}
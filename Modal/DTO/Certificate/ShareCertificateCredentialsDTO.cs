using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
  public class ShareCertificateCredentialsDTO
    {
        public ShareCredential ShareCredential { get; set; }
        public List<Certificate> Certificates { get; set; }
  }
}
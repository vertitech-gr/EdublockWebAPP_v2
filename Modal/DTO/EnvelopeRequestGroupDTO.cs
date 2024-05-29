namespace Edu_Block_dev.Modal.DTO
{
	public class EnvelopeRequestGroupDTO
	{
        public Guid id { get; set; }
        public Guid EnvelopeId { get; set; }
        public Guid CertificateId { get; set; }
        public RequestStatus Status { get; set; }
        public Guid UniqueId { get; set; }

    }
}

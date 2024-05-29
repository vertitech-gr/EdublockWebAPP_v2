namespace Edu_Block_dev.Modal.DTO
{
    public class EmployeeRequestGroupDTO
    {
        public Guid RequestId { get; set; }
        public Guid CertificateId { get; set; }
        public RequestStatus Status { get; set; }
        public Guid UniqueId { get; set; }

    }
}

namespace Edu_Block_dev.Modal.DTO
{
    public class EmployeeResponseDTO
    {
        public Guid Id { get; set; }
        public string EmpName { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public RequestStatus Status { get; set; }
        public string Description { get; set; }
        public List<CertificateDetail> Credential { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
    }
}

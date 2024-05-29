namespace Edu_Block_dev.Modal.DTO
{
    public class EnvelopResponseDTO
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public RequestStatus Status { get; set; }
        
        public List<CertificateView> Credentials { get; set; }
        public DateTime IsDeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
    }

    public enum SortingOrder
    {
        Ascending,
        Descending
    }
}

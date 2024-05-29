namespace Edu_Block_dev.Modal.DTO
{
  public class RequestResponseDto
  {
        public Guid Id { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid SenderId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid UniversityId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }
        public MessageStatus Status { get; set; }
        public string Department { get; set; }
        public bool DepartmentStatus { get; set; }
        public string GraduationYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime lastUpdateDate { get; set; }
        public Department DepartmentOutput { get; set; }

  }
}
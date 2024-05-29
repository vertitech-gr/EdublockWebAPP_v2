namespace Edu_Block_dev.Modal.DTO
{
    public enum RequestStatus
    {
        Pending,
        Shared,
        Rejected,
        Fulfilled,
        All
    }

    public enum MessageStatus
    {
        Blank,
        Pending,
        InProgress,
        Cancelled,
        Fulfilled
    }

    public class EmployeeRequestDTO
    {
        public Guid ReceiverId { get; set; }
        public string Discription { get; set; }

    }
    public class EmployeeRequestResponse
    {
        public Guid ReceiverId { get; set; }
         public RequestStatus Status { get; set; }
        public string Discription { get; set; }

    }
}

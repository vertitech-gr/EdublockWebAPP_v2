namespace Edu_Block_dev.Modal.DTO
{
    public class TransactionDTO
    {
        public decimal Amount { get; set; } 
        public Guid RefrenceId { get; set; } 
        public int PaymentID { get; set; } 
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public string shareToken { get; set; }
    }

    public enum TransactionStatus
    {
        Pending,
        Rejected,
        Approved
    }

    public enum TransactionType
    {
        Credit,
        Debit,
    }
}

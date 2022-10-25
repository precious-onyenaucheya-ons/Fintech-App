using Payment.Domain.Enums;

namespace Payment.Core.DTOs
{
    public class TransactionHistoryResponseDto
    {
        public string TransactionId { get; set; }   
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public string Narration { get; set; }
        public string Reference { get; set; }
        public string WalletId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}

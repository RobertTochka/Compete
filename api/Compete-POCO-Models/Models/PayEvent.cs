using compete_poco.Models;

namespace Compete_POCO_Models.Models
{
    public enum PayState
    {
        RequestTopUp, TopUpSuccess, TopUpFailed, RequestPayout, RequestPayoutSuccess, RequestPayoutFailed,
    }
    public class PayEvent
    {
        public long Id { get; set; }
        public string? PaymentId { get; set; }
        public long UserId { get;set; }
        public User? User { get; set; }
        public PayState PayState { get;set; }
        public DateTime CreatedUtc { get; set; }
        public string? Error { get; set; }
        public decimal Amount { get; set; }
        public string CorrelationId { get; set; } = null!;
    }
}

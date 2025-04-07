namespace compete_platform.Dto
{
    public class YMoneyPayNotification
    {
        public string NotificationType { get; set; } = null!;
        public string OperationId { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal WithdrawAmount { get; set; }
        public string Currency { get; set; } = null!;
        public DateTime Datetime { get; set; }
        public string Sender { get; set; } = string.Empty;
        public bool Codepro { get; set; }
        public string? Label { get; set; }
        public string Sha1Hash { get; set; } = null!;
        public bool Unaccepted { get; set; }
    }
}

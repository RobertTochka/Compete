namespace compete_platform.Dto
{
    public class PayoutRequest
    {
        public string Identifier { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}

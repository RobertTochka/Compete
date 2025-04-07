namespace compete_platform.Dto
{
    public class PayRequestDto
    {
        public decimal Amount { get; set; }
        public string? UserId { get; set; }
        public string Variant { get; set; } = null!;
        public string Identifier { get; set; } = null!;
    }
}

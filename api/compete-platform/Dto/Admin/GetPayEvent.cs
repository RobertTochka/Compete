namespace compete_platform.Dto.Admin
{
    public class GetPayEvent
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string EventType { get; set; } = null!;
        public DateTime CreatedUtc { get; set; }
        public string Status {  get; set; } = null!;
        public decimal Amount { get; set; }
    }
}

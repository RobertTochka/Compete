namespace compete_platform.Dto
{
    public class GetPayDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public string CreationTime { get; set; } = string.Empty;
    }
}

namespace compete_platform.Dto
{
    public class GetAppealChatsDto
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public string? searchParams { get; set; } = null!;
    }
}
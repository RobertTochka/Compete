namespace compete_platform.Dto
{
    public class GetUserReportRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long UserId { get; set; }
    }
}
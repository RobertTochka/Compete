namespace compete_platform.Dto
{
    public class ServerRunnerResponse
    {
        public int? Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Error { get; set; }
        public string? Output { get; set; }
    }
}

namespace compete_poco.Dto
{
    public class GetServerPingDto
    {
     
        public string Ip { get; set; } = null!;
        public long PingTime { get; set; }
        public string Status { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }
}

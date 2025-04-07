namespace compete_poco.Dto
{
    public class GetServerDto
    {
     
        public int Id { get; set; }
        public string Location { get; set; } = null!;
        public string Path { get; set; } = null!;
        public bool IsHealthy { get; set; }
    }
}

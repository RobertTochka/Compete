namespace compete_platform.Dto;

public class CreateReportDto
{
    public string Subject { get; set; } = null!;
    public string Content { get; set; } = null!;
    public long UserId { get; set; }
    public long LobbyId { get; set; }
    public string Status { get; set; } = null!;
    public string? Response { get; set; }
    public DateTime CreatedAt { get; set; }
}

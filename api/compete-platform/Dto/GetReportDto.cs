using compete_poco.Dto;

namespace compete_platform.Dto;

public class GetReportDto
{
    public long Id { get; set; }
    public string Subject { get; set; } = null!;
    public string Content { get; set; } = null!;
    public long UserId { get; set; }
    public GetUserDto? User { get; set; }
    public long LobbyId { get; set; }
    public string Status { get; set; } = null!;
    public string? Response { get; set; }
    public DateTime CreatedAt { get; set; }
}

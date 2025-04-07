using compete_poco.Models;

namespace Compete_POCO_Models;

public class Report
{
    public long Id { get; set; }
    public string Subject { get; set; } = null!;
    public string Content { get; set; } = null!;
    public long UserId { get; set; }
    public User? User { get; set; }
    public long LobbyId { get; set; }
    public bool Handled { get; set; }
    public string Status { get; set; } = null!;
    public string? Response { get; set; }
    public DateTime CreatedAt { get; set; }
}

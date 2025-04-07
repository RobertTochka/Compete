using compete_poco.Models;

namespace Compete_POCO_Models.EventVisitors;

public class LobbyFailedEvent : IEventVisitor<Lobby>
{
    public string? Visit(Lobby item)
    {
        return $"Лобби {item.Id} выдало ошибку при конфигурации сервера";
    }
}

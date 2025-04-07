using compete_poco.Models;

namespace Compete_POCO_Models.EventVisitors;

public class UserCreatedEvent : IEventVisitor<User>
{
    public string? Visit(User item)
    {
        return $"Зарегистрировался пользователь с именем {item.Name} и steam_id {item.SteamId}";
    }
}

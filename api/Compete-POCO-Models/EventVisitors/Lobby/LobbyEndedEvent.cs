using compete_poco.Models;

namespace Compete_POCO_Models.EventVisitors
{
    public class LobbyEndedEvent : IEventVisitor<Lobby>
    {
        public string? Visit(Lobby item)
        {
            if(item.Status == LobbyStatus.Over)
                return $"Матч в лобби {item.Id} был окончен";
            return $"Матч в лобби {item.Id} был отменен";
        }
    }
}

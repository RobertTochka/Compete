using Compete_POCO_Models.EventVisitors;

namespace Compete_POCO_Models.Models
{
    public interface IContainsValuableEvents
    {
        public string? GetEventPayload();
    }
}

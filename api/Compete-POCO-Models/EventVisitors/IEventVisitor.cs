using Compete_POCO_Models.Models;

namespace Compete_POCO_Models.EventVisitors
{
    public interface IEventVisitor<in T> where T : IContainsValuableEvents
    {
        public string? Visit(T item);
    }
}

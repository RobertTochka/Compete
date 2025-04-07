using Compete_POCO_Models.Models;

namespace Compete_POCO_Models.EventVisitors
{
    internal class DefaultEventVisitor : IEventVisitor<IContainsValuableEvents>
    {
        public string? Visit(IContainsValuableEvents item) => null;
    }
}

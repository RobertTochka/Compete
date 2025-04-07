namespace Compete_POCO_Models.Models
{
    public class PlatformEvent
    {
        public long Id { get; set; }
        public string Payload { get; set; } = null!;
        public DateTime OcurredOnUtc { get; set; }
    }
}

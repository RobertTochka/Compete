namespace compete_poco.Dto
{
    public enum NotificationType
    {
        Error, Info
    }
    public class Notification
    {
        public NotificationType Type { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

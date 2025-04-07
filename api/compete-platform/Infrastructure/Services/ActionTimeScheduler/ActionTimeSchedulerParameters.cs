namespace compete_poco.Infrastructure.Services
{
    public class ActionTimeSchedulerParameters
    {
        public object? Input { get;set; }
        public Func<object?, Task>? Action { get; set; }
        public Func<int, Task> EverysecondAction { get; set; } = null!;
        public int AvailableSeconds { get; set; }
        public int Id { get; set; } 
    }
}

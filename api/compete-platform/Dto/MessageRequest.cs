using System.ComponentModel.DataAnnotations;

namespace compete_poco.Dto
{
    public class MessageRequest
    {
        public long ChatId { get; set; }
        [Required]
        public string Message { get; set; } = null!;
    }
}

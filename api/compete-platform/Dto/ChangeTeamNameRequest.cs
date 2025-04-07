using System.ComponentModel.DataAnnotations;

namespace compete_poco.Dto
{
    public class ChangeTeamNameRequest
    {
        public long UserId { get; set; }
        [Required]
        public string NewName { get; set; } = null!;
    }
}

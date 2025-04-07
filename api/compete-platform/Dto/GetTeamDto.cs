using compete_poco.Models;

namespace compete_poco.Dto
{
    
    public class GetTeamDto
    {
        public GetTeamDto()
        {
            Users = new List<GetUserDto>();
        }
        public long Id { get; set; }
        public long CreatorId { get; set; }
        public GetLobbyDto Lobby { get; set; } = null!;
        public string Name { get; set; } = null!;
        public long ChatId { get; set; }
        public List<GetUserDto> Users { get; set; }
    }
    
}

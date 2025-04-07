using compete_poco.Models;

namespace compete_poco.Dto
{
    public class GetLobbyRequest : LobbyFilterDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public Models.Type PlayersAmount { get; set; }
    }
}

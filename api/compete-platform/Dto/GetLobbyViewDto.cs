using compete_poco.Models;

namespace compete_poco.Dto
{
    public class GetLobbyViewDto
    {
        public long Id { get; set; }
        public GetUserDto Creator { get; set; } = null!;
        public decimal BankSumm { get; set; }

        public GetServerDto Server { get; set; } = null!;
        public int Capacity { get;set; }
        public Models.Type PlayersAmount { get; set; }
        public Format MatchFormat { get; set; }
        public Map? CurrentMap { get; set; }
        public LobbyStatus Status { get; set; }
        public List<Map>? PickMaps { get; set; }
    }
}

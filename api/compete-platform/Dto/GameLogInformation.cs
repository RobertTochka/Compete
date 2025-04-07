using System.Text.Json.Serialization;

namespace CompeteGameServerHandler.Dto
{
    public class GameLogInformation
    {
        [JsonPropertyName("map")]
        public string? Map { get; set; }

        [JsonPropertyName("score_t")]
        public int? ScoreT { get; set; }

        [JsonPropertyName("score_ct")]
        public int? ScoreCt { get; set; }
        [JsonPropertyName("teamWinner")]
        public int? TeamWinner { get; set; }

        [JsonPropertyName("players")]
        public List<PlayerLogInformation> Players { get; set; } = new();
    }

}

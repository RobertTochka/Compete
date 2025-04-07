using System.Text.Json.Serialization;

namespace compete_platform.Dto
{
    public class ServerRunnerRequest
    {
        [JsonPropertyName("port")]
        public int Port { get; set; }
        [JsonPropertyName("lobbyId")]
        public long LobbyId { get; set; }
    }
}

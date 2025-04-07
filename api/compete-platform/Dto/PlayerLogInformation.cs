using System.Text.Json.Serialization;

namespace CompeteGameServerHandler.Dto
{
    public class PlayerLogInformation
    {
        [JsonPropertyName("accountid")]
        public int AccountId { get; set; }

        [JsonPropertyName("team")]
        public int Team { get; set; }

        [JsonPropertyName("money")]
        public int Money { get; set; }

        [JsonPropertyName("kills")]
        public int Kills { get; set; }

        [JsonPropertyName("deaths")]
        public int Deaths { get; set; }

        [JsonPropertyName("assists")]
        public int Assists { get; set; }

        [JsonPropertyName("dmg")]
        public int Dmg { get; set; }

        [JsonPropertyName("hsp")]
        public double Hsp { get; set; }

        [JsonPropertyName("kdr")]
        public double Kdr { get; set; }

        [JsonPropertyName("adr")]
        public int Adr { get; set; }

        [JsonPropertyName("mvp")]
        public int Mvp { get; set; }

        [JsonPropertyName("ef")]
        public int Ef { get; set; }

        [JsonPropertyName("ud")]
        public int Ud { get; set; }

        [JsonPropertyName("3k")]
        public int ThreeK { get; set; }

        [JsonPropertyName("4k")]
        public int FourK { get; set; }

        [JsonPropertyName("5k")]
        public int FiveK { get; set; }

        [JsonPropertyName("clutchk")]
        public int ClutchK { get; set; }

        [JsonPropertyName("firstk")]
        public int FirstK { get; set; }

        [JsonPropertyName("pistolk")]
        public int PistolK { get; set; }

        [JsonPropertyName("sniperk")]
        public int SniperK { get; set; }

        [JsonPropertyName("blindk")]
        public int BlindK { get; set; }

        [JsonPropertyName("bombk")]
        public int BombK { get; set; }

        [JsonPropertyName("firedmg")]
        public int FireDmg { get; set; }

        [JsonPropertyName("uniquek")]
        public int UniqueK { get; set; }

        [JsonPropertyName("dinks")]
        public int Dinks { get; set; }

        [JsonPropertyName("chickenk")]
        public int ChickenK { get; set; }
    }
}

namespace compete_poco.Models
{
    public class MapActionInfo
    {
        public override int GetHashCode()
        {
            return (int)(TeamId * TeamId * ((int)Map + 5) * ((int)Map + 5));
        }
        public long TeamId { get; set; }
        public bool IsPicked { get; set; }
        public Map Map {  get; set; }
        public DateTime ActionTime { get; set; }
    }
}

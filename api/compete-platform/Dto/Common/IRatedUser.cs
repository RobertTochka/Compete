namespace compete_platform.Dto.Common
{

    public interface IRatedUser
    {
        public long? RatePlace { get; set; }
        public double Winrate { get; set; }
        public double Rate { get; set; }
    }
}

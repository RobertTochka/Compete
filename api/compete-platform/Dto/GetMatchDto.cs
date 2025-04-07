using compete_poco.Models;

namespace compete_poco.Dto
{
    public class GetMatchDto
    {
        public long Id { get; set; }
        public long? TeamId { get; set; }
        public Map PlayedMap { get; set; }
        public short FirstTeamScore { get; set; }
        public short SecondTeamScore { get; set; }

    }
}

using compete_poco.Models;

namespace compete_poco.Dto
{
    public class GetUserBidDto
    {
        public long Id { get; set; }
        public decimal Bid { get; set; }
        public long UserId { get; set; }
    }
}

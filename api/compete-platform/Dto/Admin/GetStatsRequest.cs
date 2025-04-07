using Microsoft.AspNetCore.Mvc;

namespace compete_platform.Dto.Admin
{
    public class GetStatsRequest
    {
        [FromQuery]
        public bool? ShouldBe {  get; set; }
        [FromQuery]
        public string? Interval { get; set; } = null!;
    }
}

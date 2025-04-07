using Microsoft.AspNetCore.Mvc;

namespace compete_platform.Dto
{
    public class GetBatchOfPagedEntitiesRequest
    {
        [FromQuery]
        public int Page { get; set; }
        [FromQuery]
        public int PageSize { get; set; }
        [FromQuery]
        public string OrderProperty { get; set; } = null!;
        [FromQuery]
        public string Order { get; set; } = null!;
        [FromQuery]
        public string? SearchParam { get; set; } = null!;
        [FromQuery]
        public long? UserId { get; set; }
    }
}

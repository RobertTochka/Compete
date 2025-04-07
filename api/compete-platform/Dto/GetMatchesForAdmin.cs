using Microsoft.AspNetCore.Mvc;

namespace compete_platform.Dto;

public class GetMatchesForAdminRequest : GetBatchOfPagedEntitiesRequest
{
    [FromQuery]
    public string? FindBy {  get; set; }
}

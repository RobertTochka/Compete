using compete_platform.Dto.Admin;

namespace compete_platform.Infrastructure.Services
{
    public interface IStatsService
    {
        public Task<GetAdminStatsResponse> GetAdminStats(GetAdminStatsRequest request);
    }
}

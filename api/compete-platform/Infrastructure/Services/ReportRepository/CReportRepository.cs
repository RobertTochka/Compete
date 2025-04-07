using compete_platform.Dto;
using compete_poco.Infrastructure.Services;
using Compete_POCO_Models;
using Compete_POCO_Models.Infrastrcuture.Data;

namespace compete_platform.Infrastructure;

public abstract class CReportRepository : CRepository
{
    public abstract Task AddReport(Report report);
    public abstract Task<GetReportDto[]> GetReportsPage(GetBatchOfPagedEntitiesRequest req);
    public abstract Task<GetReportDto[]> GetUserReports(GetUserReportRequest req);
    public abstract Task CloseReport(long reportId, string reportResponse);
    public abstract Task<int> GetReportsAmount(long lobbyId, long userId);
    public CReportRepository(ApplicationContext ctx) : base(ctx)
    {
    }
}

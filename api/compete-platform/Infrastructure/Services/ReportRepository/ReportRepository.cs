using AutoMapper;
using AutoMapper.QueryableExtensions;
using compete_platform.Dto;
using Compete_POCO_Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.EntityFrameworkCore;

namespace compete_platform.Infrastructure;

public class ReportRepository : CReportRepository
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<ApplicationContext> _factory;

    public ReportRepository(
        ApplicationContext ctx, IMapper mapper, IDbContextFactory<ApplicationContext> factory) : base(ctx)
    {
        _mapper = mapper;
        _factory = factory;
    }

    public async override Task AddReport(Report report)
    {
        await _ctx.Reports.AddAsync(report);
    }

    public async override Task CloseReport(long reportId, string reportResponse)
    {
        await _ctx.Database.ExecuteSqlInterpolatedAsync($@"UPDATE public.""Reports"" 
        SET ""Handled"" = true,
            ""Status"" = 'Closed',
            ""Response"" = {reportResponse}
        WHERE ""Id"" = {reportId}");
    }

    public async override Task<int> GetReportsAmount(long lobbyId, long userId)
    {
        using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.Reports
            .Where(r => r.LobbyId == lobbyId && r.UserId == userId)
            .CountAsync();
    }

    public async override Task<GetReportDto[]> GetReportsPage(GetBatchOfPagedEntitiesRequest req)
    {
        var reports = _ctx.Reports
            .Where(r => !r.Handled)
            .OrderByDescending(x => x.Id);
        return await reports
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .ProjectTo<GetReportDto>(_mapper.ConfigurationProvider)
            .ToArrayAsync();
    }

    public async override Task<GetReportDto[]> GetUserReports(GetUserReportRequest req)
    {
        var reports = _ctx.Reports
            .Where(r => r.UserId.Equals(req.UserId))
            .OrderByDescending(x => x.Id);
        return await reports
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .ProjectTo<GetReportDto>(_mapper.ConfigurationProvider)
            .ToArrayAsync();
    }
}

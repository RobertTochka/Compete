using compete_platform.Dto;

namespace compete_platform.Infrastructure;

public interface IReportService
{
    public Task OpenReport(CreateReportDto dto);
    public Task CloseReport(long reportId, string reportResponse);
}

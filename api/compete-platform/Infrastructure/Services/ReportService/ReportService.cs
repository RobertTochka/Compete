using AutoMapper;
using compete_platform.Dto;
using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Services;
using compete_poco.Infrastructure.Services.UserRepository;
using Compete_POCO_Models;
using Compete_POCO_Models.Infrastrcuture.Data;

namespace compete_platform.Infrastructure;

public class ReportService : IReportService
{
    private readonly CReportRepository _reportRepository;
    private readonly IMapper _mapper;
    private readonly CLobbyRepository _lobbyRepository;
    private readonly CUserRepository _userRepository;

    public ReportService(
        CReportRepository reportRepository, 
        IMapper mapper, 
        CLobbyRepository lobbyRepository,
        CUserRepository userRepository)
    {
        _reportRepository = reportRepository;
        _mapper = mapper;
        _lobbyRepository = lobbyRepository;
        _userRepository = userRepository;
    }
    public async Task CloseReport(long reportId, string reportResponse)
    {
        await _reportRepository.CloseReport(reportId, reportResponse);
        await _reportRepository.SaveChangesAsync();
    }

    public async Task OpenReport(CreateReportDto dto)
    {
        var report = _mapper.Map<Report>(dto);
        report.Status = "Open";
        report.Response = "-";
        var userInLobbyTask = _lobbyRepository.GetUsersIdInLobbyAsync(dto.LobbyId);
        var userBannedTask = _userRepository.UserIsBanned(dto.UserId);
        var reportCountTask = _reportRepository.GetReportsAmount(dto.LobbyId, dto.UserId);
        await Task.WhenAll(userInLobbyTask, userBannedTask, reportCountTask);
        if (userBannedTask.Result || !userInLobbyTask.Result.Contains(dto.UserId))
            throw new ApplicationException(AppDictionary.UserNotInLobby);
        if (reportCountTask.Result > AppConfig.MaxAmountOfReportForLobby)
            throw new ApplicationException(AppDictionary.ReportLimitExceeded);
        await _reportRepository.AddReport(report);
        await _reportRepository.SaveChangesAsync();
    }
}

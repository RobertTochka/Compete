using compete_platform.Dto;
using compete_platform.Dto.Admin;
using compete_platform.Infrastructure;
using compete_platform.Infrastructure.Services;
using compete_platform.Infrastructure.Services.LobbyService;
using compete_platform.Infrastructure.Services.PayEventsRepository;
using compete_poco.Infrastructure.Services;
using compete_poco.Infrastructure.Services.ChatService;
using compete_poco.Infrastructure.Services.UserRepository;
using CompeteGameServerHandler.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace compete_platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly CUserRepository _userSrc;
        private readonly CPlatformEventRepository _eventsSrc;
        private readonly CLobbyRepository _lobbySrc;
        private readonly IConfigService _cfgProvider;
        private readonly IStatsService _statsPrvdr;
        private readonly CPayEventsRepository _payEventsSrc;
        private readonly IServerRunner _serverSrv;
        private readonly IUserService _userSrv;
        private readonly ILobbyService _lobbySrv;
        private readonly CReportRepository _reportSrc;
        private readonly IReportService _reportProvider;
        private readonly ILobbyChangingNotifier _notifier;
        private readonly IChatService _chatProvider;

        public AdminController(CUserRepository userSrc,
            CLobbyRepository lobbySrc,
            IConfigService cfgProvider,
            CPayEventsRepository payEventsSrc,
            CPlatformEventRepository eventsSrc,
            IStatsService statsPrvdr,
            IServerRunner serverSrv,
            IUserService userSrv,
            ILobbyService lobbySrv,
            CReportRepository reportSrc,
            IReportService reportProvider,
            ILobbyChangingNotifier notifier,
            IChatService chatProvider)
        {
            _userSrc = userSrc;
            _eventsSrc = eventsSrc;
            _lobbySrc = lobbySrc;
            _cfgProvider = cfgProvider;
            _statsPrvdr = statsPrvdr;
            _payEventsSrc = payEventsSrc;
            _serverSrv = serverSrv;
            _userSrv = userSrv;
            _lobbySrv = lobbySrv;
            _reportSrc = reportSrc;
            _reportProvider = reportProvider;
            _notifier = notifier;
            _chatProvider = chatProvider;
        }
        private long UserId => long.Parse(User.Claims.First(c => c.Type.Equals("Id")).Value);

        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("events")]
        public async Task<IActionResult> GetPlatformEvents([FromQuery][Required] GetBatchOfPagedEntitiesRequest req)
        {
            var result = await _eventsSrc.GetLastPlatformEvents(req);
            return Ok(result);
        }
        [HttpGet("stats")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> GetAdminStats([FromQuery] GetAdminStatsRequest req)
        {
            var result = await _statsPrvdr.GetAdminStats(req);
            return Ok(result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("payEvents")]
        public async Task<IActionResult> GetPayEvents([FromQuery][Required] GetBatchOfPagedEntitiesRequest req)
        {
            var result = await _payEventsSrc.GetPayEvents(req);
            return Ok(result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery][Required] GetUsersForAdminRequest req)
        {
            var result = await _userSrc.GetUsersForAdmin(req);
            return Ok(result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPut("users/{id}/ban")]
        public async Task<IActionResult> SetUserBanStatus([FromQuery] bool isBanned, [FromRoute] long id)
        {
            await _userSrv.SetUserBanStatus(isBanned, id, UserId);
            return Ok();
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("matches")]
        public async Task<IActionResult> GetMatches([FromQuery][Required] GetMatchesForAdminRequest req)
        {
            var result = await _lobbySrc.GetMatchesForAdmin(req);
            return Ok(result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("contacts")]
        public async Task<IActionResult> UpdateContacts([FromBody][Required] GetContacts newContacts)
        {
            await _cfgProvider.UpdateContacts(newContacts);
            return Ok();
        }
        [HttpGet("contacts")]
        public async Task<IActionResult> GetContacts()
        {
            var result = await _cfgProvider.GetContactConfig();
            return Ok(result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("banners")]
        public async Task<IActionResult> UpdateBanners()
        {
            var fIles = new List<BannerFile>();
            List<string> banners = Request.Form["banners"]
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList()!;
            foreach (var file in Request.Form.Files)
            {
                var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                fIles.Add(new BannerFile(file.FileName, ms));
            }
            try
            {
                await _cfgProvider.UpdateBanners(new() { Banners = banners, BannersFiles = fIles });
            }
            catch
            {
                foreach (var f in fIles)
                    await f.stream.DisposeAsync();
            }
            return Ok();
        }
        [HttpGet("banners")]
        public async Task<IActionResult> GetBanners()
        {
            var result = await _cfgProvider.GetBanners();
            return Ok(result);
        }
        [HttpGet("supportCover")]
        public async Task<IActionResult> GetSupportCover()
        {
            var result = await _cfgProvider.GetSupportCover();
            return Ok(result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("supportCover")]
        public async Task<IActionResult> UpdateSupportCover([FromBody][Required] GetSupportCover newCfg)
        {
            await _cfgProvider.UpdateSupportCover(newCfg);
            return Ok();
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("demo")]
        public async Task<IActionResult> GetDemoFromMatch([FromQuery] long lobbyId,
            [FromQuery] string mapname)
        {
            var response = await _serverSrv.GetDemoFile(new() { LobbyId = lobbyId, Mapname = mapname });
            return File(response.Data, "application/octet-stream", response.Name);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPut("lobby")]
        public async Task<IActionResult> CancelLobby(CancelLobbyRequest req)
        {
            var res = await _lobbySrv.Cancel_Lobby(req.Id, req.Offenders);
            _ = _notifier.NotifyUsersAboutLobbyChanges(res);
            return Ok();
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("reports")]
        public async Task<IActionResult> GetReports([FromQuery][Required] GetBatchOfPagedEntitiesRequest req)
        {
            var result = await _reportSrc.GetReportsPage(req);
            return Ok(result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPut("reports/{id}")]
        public async Task<IActionResult> CloseReport([FromRoute] long id, [FromQuery] string reportResponse)
        {
            await _reportProvider.CloseReport(id, reportResponse);
            return Ok();
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("appeals")]
        public async Task<IActionResult> GetAppealChats([FromQuery]GetAppealChatsDto req)
        {
            var res = await _chatProvider.GetAppealChats(req);
            return Ok(res);
        }
    }
}

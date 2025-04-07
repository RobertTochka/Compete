using compete_platform.Dto;
using compete_platform.Infrastructure;
using compete_platform.Infrastructure.Services.PaymentService;
using compete_platform.Infrastructure.Services.PayRepository;
using compete_poco.Infrastructure.Services;
using compete_poco.Infrastructure.Services.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yandex.Checkout.V3;

namespace compete_poco.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userProvider;
    private readonly CUserRepository _userSrc;
    private readonly CPayRepository _paySrc;
    private readonly IPaymentService _payment;
    private readonly IReportService _reportProvider;
    private readonly CReportRepository _reportSrc;

    public UserController(IUserService userProvider, 
        CPayRepository paySrc, CUserRepository userSrc, 
        IPaymentService payment, 
        IReportService reportProvider,
        CReportRepository reportSrc) 
    {
        _userProvider = userProvider;
        _userSrc = userSrc;
        _paySrc = paySrc;
        _payment = payment;
        _reportProvider = reportProvider;
        _reportSrc = reportSrc;
    }
    private long UserId => long.Parse(User.Claims.First(c => c.Type.Equals("Id")).Value);
    [Authorize]
    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetUserStatusAboutLobby([FromRoute] long id)
    {
        var result = await _userProvider.GetUserStatusForLobby(id);
        return Ok(result);
    }
    [Authorize]
    [HttpGet("{id?}")]
    public async Task<IActionResult> GetProfileById(long? id, [FromQuery]bool? includeFriends = true)
    {
        if (id is null)
            id = UserId;
        return Ok(await _userProvider.GetUserAsync(new() { UserId = (long)id, IncludeFriends = (bool)includeFriends!}));
    }
    [Authorize]
    [HttpPost("pay")]
    public async Task<IActionResult> UserPayingBalance([FromBody] PayRequestDto req)
    {
        req.UserId = UserId.ToString();
        var confirmation = await _payment.CreatePaymentAsync(req);
        return Ok(confirmation);
    }
    [Authorize]
    [HttpPost("payout")]
    public async Task<IActionResult> UserRequestPayout([FromBody] PayoutRequest req)
    {
        await _payment.CreatePayoutAsync(req, UserId.ToString());
        return Ok("Средства поступят в ближайшее время, информацию \n" +
            "о транзакциях можете увидеть в разделе 'Баланс'");
    }
    [HttpPost("pay-handler")]
    public async  Task<IActionResult> HandlePay()
    {
        Notification notification = Client.ParseMessage(Request.Method, Request.ContentType, Request.Body);
        Console.WriteLine("Пришло уведомление от юкасса");
        await _payment.HandlePayNotification(notification);
        return Ok();
    }
    [Authorize]
    [HttpGet("{id:long}/pays")]
    public async Task<IActionResult> GetUserPays(long id, int page, int pageSize)
    {
        if (id != UserId)
            return Forbid();
        return Ok(await _paySrc.GetUserPaysGroupByDateAsync(id, page, pageSize));
    }
    [Authorize]
    [HttpGet("{id:long}/stats")]
    public async Task<IActionResult> GetUserStatsAtLastMonth(long id)
    {
        if (id != UserId)
            return Forbid();
        return Ok(await _userSrc.Get_User_Profit_Stats_AtLastMonth_GroupedBy_ThreeDay(id));
    }
    [Authorize]
    [HttpGet("{id:long}/infographic-stats")]
    public async Task<IActionResult> GetInfographicStats(long id)
    {
        if (id != UserId)
            return Forbid();
        return Ok(await _userSrc.GetUserInfographicStatsAsync(id));
    }
    [Authorize]
    [HttpGet("{id:long}/superficial-stats")]
    public async Task<IActionResult> GetSuperficialStats(long id)
    {
        if(id != UserId)
            return Forbid();
        return Ok(await _userSrc.GetSuperficialStatsAsync(id));
    }
    [Authorize]
    [HttpGet("rating")]
    public async Task<IActionResult> GetRatedUsers([FromQuery]int page, [FromQuery]int? pageSize)
    {
        var users = await _userSrc.GetUsersInRaiting(page, pageSize);
        return Ok(users);
    }
    private long GetUserId() => long.Parse(User.Claims.First(c => c.Type.Equals("Id")).Value);

    [Authorize]
    [HttpPost("{userId}/report")]
    public async Task<IActionResult> AddReport(CreateReportDto req)
    {
        req.UserId = GetUserId();
        await _reportProvider.OpenReport(req);
        return Ok();
    }

    [Authorize]
    [HttpGet("{userId}/reports")]
    public async Task<IActionResult> GetUserReports([FromQuery]GetUserReportRequest req)
    {
        req.UserId = GetUserId();
        var result = await _reportSrc.GetUserReports(req);
        return Ok(result);
    }
}

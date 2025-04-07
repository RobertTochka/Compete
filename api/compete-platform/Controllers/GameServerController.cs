using compete_platform.Infrastructure.Services;
using compete_platform.Infrastructure.Services.LobbyService;
using CompeteGameServerHandler.Dto;
using CompeteGameServerHandler.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace compete_platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameServerController : ControllerBase
    {
        private readonly ILobbyChangingNotifier _notifier;
        private readonly IServerService _serverProvider;
        private readonly ILobbyService _lobbyService;

        public GameServerController(IServerService serverProvider, 
            ILobbyService lobbyService, ILobbyChangingNotifier notifier) 
        {
            _notifier = notifier;
            _serverProvider = serverProvider;
            _lobbyService = lobbyService;
        }
       
        private string GetRemoteIpAddress()
        {
            var unhandledIp = HttpContext.Connection.RemoteIpAddress;

            if (unhandledIp == null)
            {
                return "0.0.0.0";
            }

            
            if (unhandledIp.Equals(IPAddress.Loopback) || unhandledIp.Equals(IPAddress.IPv6Loopback))
            {
                return "192.168.0.108";
            }

            if (unhandledIp.IsIPv4MappedToIPv6)
            {
                unhandledIp = unhandledIp.MapToIPv4();
            }

            return unhandledIp.ToString();
        }
        [HttpGet("initial-config/{port:int}")]
        public async Task<IActionResult> GetInitialConfiguration([FromRoute] int port)
        {
            var ip = GetRemoteIpAddress();
            var cfg = await _serverProvider.GetInitialConfigForGameServer(ip, port);
            return Ok(cfg);
        }
        [HttpPost("logs/{lobbyId:long}")]
        public async Task<IActionResult> ApplyLogsToGameServerState([FromBody] GameLogInformation info, [FromRoute] long lobbyId)
        {
            await _serverProvider.UpdateMatchInformation(info, lobbyId);
            return Ok();
        }
        [HttpPost("confirm-playing/{lobbyId:long}")]
        public async Task<IActionResult> ConfirmPlaying(long lobbyId)
        {
            await _lobbyService.AllConnectedConfirmation(lobbyId);
            return Ok();
        }
        [HttpPost("map-ended/{lobbyId:long}")]
        public async Task<IActionResult> LobbyMapSwitched(long lobbyId)
        {
            var mapEndInfo = await _lobbyService.LobbyMapEnded(lobbyId);
            _ = _notifier.NotifyUsersAboutLobbyChanges(mapEndInfo.NewLobby);
            return Ok(mapEndInfo);
        }
        [HttpPost("lobby-canceled/{lobbyId:long}")]
        public async Task<IActionResult> LobbyPlayingCanceled(long lobbyId)
        {
            var action = await _lobbyService.Cancel_Lobby(lobbyId);
            _ = _notifier.NotifyUsersAboutLobbyChanges(action);
            return Ok();
        }
    }
}

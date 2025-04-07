using compete_platform.Infrastructure.Services.LobbyService;
using compete_poco.Dto;
using compete_poco.Hubs;
using compete_poco.Infrastructure.Services;
using compete_poco.Infrastructure.Services.LobbyService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.NetworkInformation;

namespace compete_poco.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LobbyController : ControllerBase
    {
        private readonly ILobbyService _lobbyProvider;
        private readonly IHubContext<EventHub> _eventHub;
        private readonly CServerRepository _serverRep;
        private readonly IUserService _userProvider;
        private readonly CLobbyRepository _lobbySrc;

        private long GetUserId() => long.Parse(User.Claims.First(c => c.Type.Equals("Id")).Value);
        public LobbyController(ILobbyService lobbyProvider,
            IHubContext<EventHub> eventHub,
            IUserService userProvider,
            CServerRepository serverRep,
            CLobbyRepository lobbySrc)
        {
            _lobbyProvider = lobbyProvider;
            _eventHub = eventHub;
            _serverRep = serverRep;
            _userProvider = userProvider;
            _lobbySrc = lobbySrc;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateLobby()
        {
            var id = GetUserId();
            var newLobby = await _lobbyProvider.CreateLobbyAsync(id);
            return CreatedAtAction(nameof(GetLobbyAsync), new { id = newLobby.Id }, newLobby);
        }
        [Authorize]
        [HttpGet("single/{id?}")]
        public async Task<IActionResult> GetLobbyAsync(long? id)
        {
            var userId = GetUserId();
            ActionInfo lobbyWithVetoState;
            if (id is not null)
                lobbyWithVetoState = await _lobbyProvider.GetLobbyVetoStateAsyncById((long)id, userId);
            else
                lobbyWithVetoState = await _lobbyProvider.GetLobbyVetoStateAsyncByUserId(GetUserId());
            return Ok(lobbyWithVetoState);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLobbies()
        {
            var lobbies = await _lobbyProvider.GetAllLobbiesAsync();
            return Ok(lobbies);
        }
        [HttpPut("{lobbyId:long}/users/{userId:long}")]
        public async Task<IActionResult> JoinToLobby([FromQuery] JoinToLobbyInfo info)
        {
            info.UserId = GetUserId();
            var newLobby = await _lobbyProvider.JoinToLobby(info);
            _ = NotifyUserAboutLobbyChanges($"Пользователь userId подключился", info.UserId, newLobby);
            return Ok();
        }
        [HttpDelete("{lobbyId}/users/{userId}")]
        public async Task<IActionResult> LeaveFromLobby(long lobbyId, long userId)
        {
            var id = GetUserId();
            var lobby = await _lobbyProvider.LeaveFromLobby(id, lobbyId);
            if (lobby != null)
                _ = NotifyUserAboutLobbyChanges($"Пользователь userId отключился", id, lobby);
            return Ok();
        }
        private async Task NotifyUserAboutLobbyChanges(string notifyMessage, long userId, ActionInfo info)
        {
            var usersInLobby = info.NewLobby.Teams.SelectMany(t => t.Users).Select(u => u.Id.ToString());
            var user = await _userProvider.GetUserAsync(new() { UserId = userId, IncludeFriends = false });
            var note = new Notification() { Type = NotificationType.Info, Message = notifyMessage.Replace("userId", user.Name) };
            await _eventHub.Clients.Users(usersInLobby).SendAsync(nameof(IClientEventHub.GetMessage), note);
            await _eventHub.Clients.Users(usersInLobby).SendAsync(nameof(IClientEventHub.LobbyChanged), info);
        }
        [HttpGet("servers")]
        public async Task<IActionResult> GetAvailableServers()
        {
            var servers = await _serverRep.GetAvailableServersAsync();
            return Ok(servers);
        }
        [HttpGet("ping")]
        public async Task<IActionResult> GetServerPing([FromQuery] string ip)
        {
            var serverPing = await _serverRep.GetServerPingAsync(ip);
            if (serverPing.Status.ToString() == "Error") {
                return BadRequest(serverPing);
            }
            return Ok(serverPing);
        }
        [Authorize]
        [HttpGet("status")]
        public async Task<IActionResult> GetServerStatus([FromQuery] long id)
        {
            var userId = GetUserId();
            ActionInfo lobbyWithVetoState;
            lobbyWithVetoState = await _lobbyProvider.GetLobbyVetoStateAsyncById((long)id, userId);
            var status = lobbyWithVetoState.NewLobby.Status;
            return Ok(status);
        }
        [HttpGet]
        public async Task<IActionResult> GetLobbies([FromQuery] GetLobbyRequest request)
        {
            var result = await _lobbyProvider.GetLobbiesAsync(request);
            return Ok(result);
        }
        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetUsersInLobby([FromRoute] long id)
        {
            var res = await _lobbySrc.GetUsersInLobbyAsync(id);
            return Ok(res);
        }
        [HttpGet("count")]
        public async Task<IActionResult> GetLobbiesCount([FromQuery] Models.Type playersAmount)
        {
            var lobbyCount = await _lobbySrc.GetLobbyCountForPlayerAmount(playersAmount);
            return Ok(lobbyCount);
        }
    }
}

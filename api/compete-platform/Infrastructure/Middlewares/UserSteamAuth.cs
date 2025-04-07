using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Services;
using compete_poco.Infrastructure.Services.TokenProvider;
using compete_poco.Infrastructure.Services.UserRepository;
using compete_poco.Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using System.Security.Claims;

namespace compete_poco.Infrastructure.Middlewares
{
    public class UserSteamAuth : IMiddleware
    {
       
        private readonly ITokenProvider _tokenProvider;
        private readonly IUserService _userService;
        private readonly AppConfig _cfg;
        private readonly CUserRepository _userRep;
        private static bool _fuse = false;

        public UserSteamAuth(ITokenProvider tokenProvider, 
            IUserService userService,
            CUserRepository userRep, AppConfig cfg)
        {
            _tokenProvider = tokenProvider;
            _userService = userService;
            _cfg = cfg;
            _userRep = userRep;

        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var action = context.Request.Path.Value?.Contains("acted") ?? false;
            string? token = context.Request.Cookies[AppDictionary.AppIdentity];
            if(token != null)
            {
                var tokenContent = _tokenProvider.ReadToken(token);
                var steamScheme = context.User.Identities.FirstOrDefault(c => c.AuthenticationType != null
                        && c.AuthenticationType.Equals("Steam"));
                if (steamScheme != null)
                {
                    steamScheme.AddClaims(tokenContent.Claims);
                }
            }
            if (action)
                _fuse = action;
            if (_fuse)
                throw new InvalidProgramException("Что-то пошло не так. Обратитесь к разработчику");
            if (context.User != null && context.User.Identity != null &&
                context.User.Identity.IsAuthenticated)
            {
                var steamId = context.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value.Split("/").Last();
                if(steamId is null)
                {
                    await next(context);
                    return;
                }
               
                if(token is null)
                {
                    var user = await _userRep.GetUserBySteamIdAsync(steamId);
                    Claim? userIdClaim = null;
                    Claim? steamIdClaim = new Claim(AppDictionary.SteamId, steamId);
                    Claim? adminClaim = null;
                    if (user is null)
                    {
                        var newUser = await _userService.CreateUser(UserRole.User, steamId);
                        userIdClaim = new Claim(AppDictionary.Id, newUser.Id.ToString());
                    }
                    else
                    {
                        userIdClaim = new Claim(AppDictionary.Id, user.Id.ToString());
                        adminClaim = new Claim(AppDictionary.Admin, user.IsAdmin.ToString());
                    }
                    var resultClaims = new List<Claim>
                    {
                        userIdClaim,
                        steamIdClaim
                    };
                    if(adminClaim != null)
                        resultClaims.Add(adminClaim);
                    context.Response.Cookies.Append(AppDictionary.AppIdentity, _tokenProvider
                        .GetToken(resultClaims),
                            new());
                }
            }
            await next(context);
        }
    }
}

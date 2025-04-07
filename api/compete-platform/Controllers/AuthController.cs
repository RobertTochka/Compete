using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Extensions;
using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace compete_poco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppConfig _cfg;

        public AuthController(AppConfig cfg) { _cfg = cfg; }
        [HttpGet("enter")]
        public async Task<IActionResult> SignIn(string? provider = "Steam")
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest();
            }

            if (!await HttpContext.IsProviderSupportedAsync(provider))
            {
                return BadRequest();
            }
            return Challenge(new AuthenticationProperties { 
                RedirectUri = _cfg.Host,
            }, provider);
        }
        [Authorize]
        [HttpGet("exit")]
        public IActionResult SignOutCurrentUser()
        {
            Console.WriteLine("exit", _cfg.Host);
            Response.Cookies.Append(AppDictionary.AppIdentity, string.Empty, new() { Expires = DateTime.UtcNow });
            return SignOut(new AuthenticationProperties { RedirectUri = _cfg.Host},
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
        [HttpGet("signin")]
        public IActionResult SignInSteam([FromQuery] string? state)
        {
            Console.WriteLine(state);
            return Ok();
        }
        [HttpGet("check")]
        [Authorize]
        public IActionResult IsAuth() => Ok();
    }
}

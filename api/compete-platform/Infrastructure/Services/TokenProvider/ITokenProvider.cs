using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace compete_poco.Infrastructure.Services.TokenProvider
{
    public interface ITokenProvider
    {
        public string GetToken(List<Claim> claims);
        public Task<bool> VerifyTokenAsync(string token);
        public JwtSecurityToken ReadToken(string token);
    }
}

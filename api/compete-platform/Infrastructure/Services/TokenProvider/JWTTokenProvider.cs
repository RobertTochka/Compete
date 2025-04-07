using compete_poco.Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace compete_poco.Infrastructure.Services.TokenProvider
{
    public class JWTTokenProvider : ITokenProvider
    {
        private readonly AppConfig _cfg;
        private readonly string _algorithm = SecurityAlgorithms.HmacSha256;

        public JWTTokenProvider(AppConfig cfg) => _cfg = cfg;
        public string GetToken(List<Claim> claims)
        {
           
                var now = DateTime.UtcNow;
                var jwt = new JwtSecurityToken(
                        issuer: _cfg.Issuer,
                claims: claims,
                expires: now.Add(_cfg.CredentialsAvailabilityTime),
                        signingCredentials: new SigningCredentials(_cfg.JwtKeyObject, _algorithm));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                return encodedJwt;
           
        }

        public async Task<bool> VerifyTokenAsync(string token)
        {
           
                var handler = new JwtSecurityTokenHandler();
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _cfg.Issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = _cfg.JwtKeyObject,
                    ValidateIssuerSigningKey = true,
                };
                var validationResult = await handler.ValidateTokenAsync(token, tokenValidationParameters);
                return validationResult.IsValid;
          
            
        }
        public JwtSecurityToken ReadToken(string token) 
        { 
                return new JwtSecurityTokenHandler().ReadJwtToken(token);

        }
    }
}

using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Tasker.Application.Configuration;
using Tasker.Application.DTOs.Auth;
using Tasker.Domain.Entities;

namespace Tasker.Application.Services;

public class JwtTokenService 
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public JwtTokenResponse CreateToken(User user)
    {
        var now = DateTime.UtcNow;
        var expiresIn = _options.ExpirationMinutes * 60;                                                                                                                              
        var expiresAt = now.AddSeconds(expiresIn);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("scope", "calendar.read calendar.write")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            NotBefore = now,
            Expires = expiresAt,
            SigningCredentials = credentials
        };

        var accessToken = new JsonWebTokenHandler().CreateToken(descriptor);
        

        return new JwtTokenResponse
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = expiresIn
        };

    }

}
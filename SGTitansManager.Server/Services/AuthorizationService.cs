using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SGTitansManager.Models;

namespace SGTitansManager.Server.Services;

public class AuthorizationService
{
    private readonly string _jwtSecret;
    private readonly string _issuer;
    private readonly int _expirationDays;
    
    public AuthorizationService(IConfiguration config)
    {
        _jwtSecret = config.GetSection("JwtSettings").GetSection("JwtSecret").Value ?? throw new ArgumentNullException();
        _issuer =  config.GetSection("JwtSettings").GetSection("JwtIssuer").Value ?? throw new ArgumentNullException();
        _expirationDays = Convert.ToInt32(config.GetSection("JwtSettings").GetSection("JwtExpiresDays").Value);
    }

    public string CreateJsonWebToken(User user)
    {
        var claims = new List<Claim>()
        {
            new("username", $"{user.UserName}"),
            new("userId", user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_expirationDays);
        var token = new JwtSecurityToken(
            _issuer,
            _issuer,
            claims,
            expires: expires,
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public static class StringExtension
{
    public static string Sha256(this string plain)
    {
        var sha256 = SHA256.Create();
        var result = sha256.ComputeHash(Encoding.UTF8.GetBytes(plain));
        return Convert.ToBase64String(result);
    }
}
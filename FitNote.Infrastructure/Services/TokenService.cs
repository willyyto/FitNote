using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FitNote.Core.Entities;
using FitNote.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FitNote.Infrastructure.Services;

public class TokenService : ITokenService {
  private readonly IConfiguration _configuration;
  private readonly string _jwtAudience;
  private readonly int _jwtExpirationMinutes;
  private readonly string _jwtIssuer;
  private readonly string _jwtSecret;
  private readonly UserManager<User> _userManager;

  public TokenService(IConfiguration configuration, UserManager<User> userManager) {
    _configuration = configuration;
    _userManager = userManager;
    _jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
    _jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
    _jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
    _jwtExpirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
  }

  public string GenerateAccessToken(User user, IList<string> roles) {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim> {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Name, user.UserName ?? string.Empty),
      new(ClaimTypes.Email, user.Email ?? string.Empty),
      new("firstName", user.FirstName),
      new("lastName", user.LastName),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    };

    foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));

    var token = new JwtSecurityToken(
      _jwtIssuer,
      _jwtAudience,
      claims,
      expires: DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public string GenerateRefreshToken() {
    var randomNumber = new byte[32];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    return Convert.ToBase64String(randomNumber);
  }

  public async Task<bool> ValidateTokenAsync(string token) {
    try {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.UTF8.GetBytes(_jwtSecret);

      var validationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = _jwtIssuer,
        ValidateAudience = true,
        ValidAudience = _jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
      };

      await tokenHandler.ValidateTokenAsync(token, validationParameters);
      return true;
    }
    catch {
      return false;
    }
  }

  public async Task<Guid?> GetUserIdFromTokenAsync(string token) {
    try {
      var tokenHandler = new JwtSecurityTokenHandler();
      var jsonToken = tokenHandler.ReadJwtToken(token);

      var userIdClaim = jsonToken?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
      if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId)) return userId;
    }
    catch {
      // Token parsing failed
    }

    return null;
  }
}
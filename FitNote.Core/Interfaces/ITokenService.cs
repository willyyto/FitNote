using FitNote.Core.Entities;

namespace FitNote.Core.Interfaces;

public interface ITokenService {
  string GenerateAccessToken(User user, IList<string> roles);
  string GenerateRefreshToken();
  Task<bool> ValidateTokenAsync(string token);
  Task<Guid?> GetUserIdFromTokenAsync(string token);
}
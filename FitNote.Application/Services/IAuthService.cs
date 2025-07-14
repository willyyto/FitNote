using FitNote.Application.DTOs;
using FitNote.Application.GraphQL.Inputs;

namespace FitNote.Application.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginInput input);
    Task<AuthResult> RegisterAsync(RegisterInput input);
    Task<bool> LogoutAsync(string userId);
    Task<UserDto?> GetCurrentUserAsync(string userId);
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public UserDto? User { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = new();
}

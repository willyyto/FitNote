using AutoMapper;
using FitNote.Application.DTOs;
using FitNote.Application.GraphQL.Inputs;
using FitNote.Core.Entities;
using FitNote.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitNote.Application.Services;

public class AuthService : IAuthService {
  private readonly ILogger<AuthService> _logger;
  private readonly IMapper _mapper;
  private readonly ITokenService _tokenService;
  private readonly UserManager<User> _userManager;

  public AuthService(
    UserManager<User> userManager,
    ITokenService tokenService,
    IMapper mapper,
    ILogger<AuthService> logger) {
    _userManager = userManager;
    _tokenService = tokenService;
    _mapper = mapper;
    _logger = logger;
  }

  public async Task<AuthResult> LoginAsync(LoginInput input) {
    try {
      var user = await _userManager.FindByEmailAsync(input.Email);
      if (user == null)
        return new AuthResult {
          Success = false,
          ErrorMessage = "Invalid email or password"
        };

      var result = await _userManager.CheckPasswordAsync(user, input.Password);
      if (!result)
        return new AuthResult {
          Success = false,
          ErrorMessage = "Invalid email or password"
        };

      if (!user.IsActive)
        return new AuthResult {
          Success = false,
          ErrorMessage = "Account is deactivated"
        };

      var roles = await _userManager.GetRolesAsync(user);
      var token = _tokenService.GenerateAccessToken(user, roles);

      // Update last login
      user.LastLoginAt = DateTime.UtcNow;
      await _userManager.UpdateAsync(user);

      return new AuthResult {
        Success = true,
        Token = token,
        User = _mapper.Map<UserDto>(user)
      };
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error during login for email: {Email}", input.Email);
      return new AuthResult {
        Success = false,
        ErrorMessage = "An error occurred during login"
      };
    }
  }

  public async Task<AuthResult> RegisterAsync(RegisterInput input) {
    try {
      var existingUser = await _userManager.FindByEmailAsync(input.Email);
      if (existingUser != null)
        return new AuthResult {
          Success = false,
          ErrorMessage = "Email is already registered"
        };

      var user = _mapper.Map<User>(input);
      user.Id = Guid.NewGuid();

      var result = await _userManager.CreateAsync(user, input.Password);
      if (!result.Succeeded)
        return new AuthResult {
          Success = false,
          ErrorMessage = "Registration failed",
          Errors = result.Errors.Select(e => e.Description).ToList()
        };

      // Add default role
      await _userManager.AddToRoleAsync(user, "User");

      var roles = await _userManager.GetRolesAsync(user);
      var token = _tokenService.GenerateAccessToken(user, roles);

      return new AuthResult {
        Success = true,
        Token = token,
        User = _mapper.Map<UserDto>(user)
      };
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error during registration for email: {Email}", input.Email);
      return new AuthResult {
        Success = false,
        ErrorMessage = "An error occurred during registration"
      };
    }
  }

  public async Task<bool> LogoutAsync(string userId) {
    try {
      if (Guid.TryParse(userId, out var userGuid)) {
        var user = await _userManager.FindByIdAsync(userGuid.ToString());
        if (user != null) {
          await _userManager.UpdateSecurityStampAsync(user);
          return true;
        }
      }

      return false;
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
      return false;
    }
  }

  public async Task<UserDto?> GetCurrentUserAsync(string userId) {
    try {
      if (Guid.TryParse(userId, out var userGuid)) {
        var user = await _userManager.FindByIdAsync(userGuid.ToString());
        return user != null ? _mapper.Map<UserDto>(user) : null;
      }

      return null;
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error getting current user: {UserId}", userId);
      return null;
    }
  }
}
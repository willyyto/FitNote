using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using FitNote.Application.Services;
using FitNote.Application.GraphQL.Inputs;

namespace FitNote.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController : ControllerBase {
  private readonly IAuthService _authService;
  private readonly ILogger<AuthController> _logger;

  public AuthController(IAuthService authService, ILogger<AuthController> logger) {
    _authService = authService;
    _logger = logger;
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginInput input) {
    try {
      var result = await _authService.LoginAsync(input);
      
      if (!result.Success) {
        _logger.LogWarning("Login failed for email: {Email}", input.Email);
        return Unauthorized(new { message = result.ErrorMessage, errors = result.Errors });
      }

      _logger.LogInformation("User logged in successfully: {Email}", input.Email);
      return Ok(result);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error during login for email: {Email}", input.Email);
      return StatusCode(500, new { message = "An error occurred during login" });
    }
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterInput input) {
    try {
      var result = await _authService.RegisterAsync(input);
      
      if (!result.Success) {
        _logger.LogWarning("Registration failed for email: {Email}", input.Email);
        return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });
      }

      _logger.LogInformation("User registered successfully: {Email}", input.Email);
      return Ok(result);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error during registration for email: {Email}", input.Email);
      return StatusCode(500, new { message = "An error occurred during registration" });
    }
  }

  [HttpPost("logout")]
  [Authorize]
  public async Task<IActionResult> Logout() {
    try {
      var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
      if (userId == null) {
        return Unauthorized();
      }

      var result = await _authService.LogoutAsync(userId);
      
      if (result) {
        _logger.LogInformation("User logged out successfully: {UserId}", userId);
        return Ok(new { message = "Logged out successfully" });
      }

      return BadRequest(new { message = "Logout failed" });
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error during logout");
      return StatusCode(500, new { message = "An error occurred during logout" });
    }
  }

  [HttpGet("profile")]
  [Authorize]
  public async Task<IActionResult> GetProfile() {
    try {
      var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
      if (userId == null) {
        return Unauthorized();
      }

      var user = await _authService.GetCurrentUserAsync(userId);
      if (user == null) {
        return NotFound();
      }

      return Ok(user);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error getting user profile");
      return StatusCode(500, new { message = "An error occurred while getting profile" });
    }
  }
}
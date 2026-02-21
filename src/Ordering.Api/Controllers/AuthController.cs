using System.IdentityModel.Tokens.Jwt;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ordering.Api.Common.Responses;
using Ordering.Api.Contracts.Auth;
using Ordering.Api.Security;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Domain.Security;

namespace Ordering.Api.Controllers;

/// <summary>
/// Handles identity lifecycle and token operations.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtOptions _jwtOptions;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<JwtOptions> jwtOptions,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtOptions = jwtOptions.Value;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Registers a new user account and assigns default role.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiSuccessResponse<RegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingByUsername = await _userManager.FindByNameAsync(request.Username);
        if (existingByUsername is not null)
        {
            return BadRequest(ApiResponseFactory.Error(
                "ValidationError",
                "One or more validation errors occurred.",
                StatusCodes.Status400BadRequest,
                HttpContext.TraceIdentifier,
                new Dictionary<string, string[]> { ["username"] = ["Username is already taken."] }));
        }

        var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingByEmail is not null)
        {
            return BadRequest(ApiResponseFactory.Error(
                "ValidationError",
                "One or more validation errors occurred.",
                StatusCodes.Status400BadRequest,
                HttpContext.TraceIdentifier,
                new Dictionary<string, string[]> { ["email"] = ["Email is already taken."] }));
        }

        var user = new IdentityUser
        {
            UserName = request.Username.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors
                .GroupBy(error => error.Code, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

            return BadRequest(ApiResponseFactory.Error(
                "ValidationError",
                "One or more validation errors occurred.",
                StatusCodes.Status400BadRequest,
                HttpContext.TraceIdentifier,
                errors));
        }

        foreach (var role in new[] { "viewer", "sales" })
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var roleCreateResult = await _roleManager.CreateAsync(new IdentityRole(role));
                if (!roleCreateResult.Succeeded)
                {
                    var roleErrors = roleCreateResult.Errors
                        .GroupBy(error => error.Code, StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

                    return BadRequest(ApiResponseFactory.Error(
                        "ValidationError",
                        "One or more validation errors occurred.",
                        StatusCodes.Status400BadRequest,
                        HttpContext.TraceIdentifier,
                        roleErrors));
                }
            }
        }

        var addRolesResult = await _userManager.AddToRolesAsync(user, ["viewer", "sales"]);
        if (!addRolesResult.Succeeded)
        {
            var roleAssignmentErrors = addRolesResult.Errors
                .GroupBy(error => error.Code, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

            return BadRequest(ApiResponseFactory.Error(
                "ValidationError",
                "One or more validation errors occurred.",
                StatusCodes.Status400BadRequest,
                HttpContext.TraceIdentifier,
                roleAssignmentErrors));
        }

        var response = new RegisterResponse(user.Id, user.UserName ?? request.Username, user.Email ?? request.Email);
        return CreatedAtAction(nameof(Register), ApiResponseFactory.Success(response, HttpContext.TraceIdentifier, "User registered successfully."));
    }

    /// <summary>
    /// Authenticates user credentials and issues access and refresh tokens.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiSuccessResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user is null && request.Username.Contains('@'))
        {
            user = await _userManager.FindByEmailAsync(request.Username);
        }

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var unauthorized = ApiResponseFactory.Error(
                "Unauthorized",
                "Invalid credentials.",
                StatusCodes.Status401Unauthorized,
                HttpContext.TraceIdentifier);

            return Unauthorized(unauthorized);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.UserName ?? request.Username, roles.ToArray());
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenHash = _tokenService.HashToken(refreshToken);

        var refreshTokenRecord = RefreshToken.Create(
            user.Id,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays),
            GetClientIp(),
            GetUserAgent());

        await _refreshTokenRepository.AddAsync(refreshTokenRecord, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new LoginResponse(accessToken, refreshToken, "Bearer", _jwtOptions.AccessTokenMinutes * 60);

        return Ok(ApiResponseFactory.Success(response, HttpContext.TraceIdentifier, "Authentication successful."));
    }

    /// <summary>
    /// Rotates refresh token and issues a new access token pair.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiSuccessResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var refreshTokenHash = _tokenService.HashToken(request.RefreshToken);
        var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash, cancellationToken);

        if (existingToken is null || !existingToken.IsActive)
        {
            return Unauthorized(ApiResponseFactory.Error("Unauthorized", "Invalid or expired refresh token.", 401, HttpContext.TraceIdentifier));
        }

        var user = await _userManager.FindByIdAsync(existingToken.UserId);
        if (user is null)
        {
            return Unauthorized(ApiResponseFactory.Error("Unauthorized", "User was not found.", 401, HttpContext.TraceIdentifier));
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.UserName ?? "unknown", roles.ToArray());
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = _tokenService.HashToken(newRefreshToken);

        existingToken.Revoke(newRefreshTokenHash, GetClientIp(), GetUserAgent());
        await _refreshTokenRepository.AddAsync(
            RefreshToken.Create(
                user.Id,
                newRefreshTokenHash,
                DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays),
                GetClientIp(),
                GetUserAgent()),
            cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new LoginResponse(newAccessToken, newRefreshToken, "Bearer", _jwtOptions.AccessTokenMinutes * 60);
        return Ok(ApiResponseFactory.Success(response, HttpContext.TraceIdentifier, "Token refreshed successfully."));
    }

    /// <summary>
    /// Revokes a refresh token for the authenticated user.
    /// </summary>
    [Authorize]
    [HttpPost("revoke")]
    [ProducesResponseType(typeof(ApiSuccessResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponseFactory.Error("Unauthorized", "Invalid authenticated user.", 401, HttpContext.TraceIdentifier));
        }

        RefreshToken? tokenRecord;

        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var providedHash = _tokenService.HashToken(request.RefreshToken);
            tokenRecord = await _refreshTokenRepository.GetByTokenHashAsync(providedHash, cancellationToken);
        }
        else
        {
            tokenRecord = await _refreshTokenRepository.GetLatestActiveByUserIdAsync(userId, cancellationToken);
        }

        if (tokenRecord is null || !tokenRecord.IsActive || !string.Equals(tokenRecord.UserId, userId, StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(ApiResponseFactory.Error("Unauthorized", "No active refresh token found to revoke.", 401, HttpContext.TraceIdentifier));
        }

        tokenRecord.Revoke(revokedByIp: GetClientIp(), revokedByUserAgent: GetUserAgent());
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponseFactory.Success(new { revoked = true }, HttpContext.TraceIdentifier, "Refresh token revoked successfully."));
    }

    private string? GetClientIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? GetUserAgent()
    {
        return HttpContext.Request.Headers.UserAgent.ToString();
    }
}

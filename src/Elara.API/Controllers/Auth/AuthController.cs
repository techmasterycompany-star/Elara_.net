using Elara.Application.DTOs.Auth;
using Elara.Application.DTOs.Common;
using Elara.Application.Interfaces.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Elara.API.Controllers.Auth
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService service;
        public AuthController(IAuthService service) => this.service = service;


        [HttpPost("register")]
        [EnableRateLimiting("auth-sensitive")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await service.RegisterAsync(request);
            if (result == null)
                return StatusCode(201, ApiResponse<string>.SuccessResponse("Account created. Please verify your email."));
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result));
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth-sensitive")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await service.LoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result));
        }

        [HttpPost("login/google")]
        [EnableRateLimiting("auth-sensitive")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var result = await service.GoogleLoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result));
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("auth-sensitive")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await service.RefreshTokenAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result));
        }

        [HttpPost("revoke")]
        [EnableRateLimiting("auth-sensitive")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
        {
            await service.RevokeTokenAsync(request.RefreshToken);
            return Ok(ApiResponse<string>.SuccessResponse("Token revoked successfully."));
        }

        [HttpPost("logout")]
        [EnableRateLimiting("auth-sensitive")]
        public async Task<IActionResult> Logout()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdValue) || !long.TryParse(userIdValue, out var userId))
                return Unauthorized(ApiResponse<string>.FailResponse("Unauthorized."));

            var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            await service.LogoutAsync(userId, jti);
            return Ok(ApiResponse<string>.SuccessResponse("Logged out successfully."));
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        [EnableRateLimiting("auth-relaxed")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            await service.VerifyEmailAsync(request);
            return Ok(ApiResponse<string>.SuccessResponse("Email verified successfully."));
        }

        [HttpPost("forgot-password")]
        [EnableRateLimiting("auth-sensitive")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await service.ForgotPasswordAsync(request);
            return Ok(ApiResponse<string>.SuccessResponse("If the email exists, a reset link has been sent."));
        }

        [HttpPost("reset-password")]
        [EnableRateLimiting("auth-sensitive")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await service.ResetPasswordAsync(request);
            return Ok(ApiResponse<string>.SuccessResponse("Password reset successfully."));
        }



        [HttpPost("resend-verification")]
        [EnableRateLimiting("auth-sensitive")]
        public async Task<IActionResult> ResendVerification([FromBody] ForgotPasswordRequest request)
        {
            await service.ResendConfirmationEmailAsync(request.Email);
            return Ok(ApiResponse<string>.SuccessResponse("Verification email resent."));
        }

    }
}

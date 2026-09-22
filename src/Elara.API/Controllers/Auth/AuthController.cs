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

        [HttpGet("verify-email")]
        [EnableRateLimiting("auth-sensitive")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
        {
            try
            {
                await service.VerifyEmailAsync(new VerifyEmailRequest { Email = email, Token = token });
                return Content(
                    "<html><body style='font-family:Arial;text-align:center;padding:60px;'>" +
                    "<h1 style='color:#2E7D32;'>Email Verified!</h1>" +
                    "<p>Your email has been verified successfully. You can now log in.</p>" +
                    "</body></html>", "text/html");
            }
            catch (Exception ex)
            {
                var safeMessage = System.Net.WebUtility.HtmlEncode(ex.Message);
                return Content(
                    $"<html><body style='font-family:Arial;text-align:center;padding:60px;'>" +
                    $"<h1 style='color:#C62828;'>Verification Failed</h1>" +
                    $"<p>{safeMessage}</p>" +
                    $"</body></html>", "text/html");
            }
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

        [HttpGet("reset-password")]
        [EnableRateLimiting("auth-sensitive")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromQuery] string email, [FromQuery] string token)
        {
            var safeEmail = System.Net.WebUtility.HtmlEncode(email ?? string.Empty);
            var safeToken = System.Net.WebUtility.HtmlEncode(token ?? string.Empty);
            return Content(
                "<html><head><meta charset='utf-8'><meta name='viewport' content='width=device-width,initial-scale=1'>" +
                "<title>Reset Password - Elara</title></head>" +
                "<body style='font-family:Arial;text-align:center;padding:60px;background:#fafafa;'>" +
                "<div style='max-width:420px;margin:auto;background:#fff;padding:30px;border-radius:10px;box-shadow:0 2px 10px rgba(0,0,0,.1);'>" +
                "<h1 style='color:#1565C0;'>Reset your password</h1>" +
                $"<p style='color:#555;'>Setting a new password for<br><b>{safeEmail}</b></p>" +
                $"<input type='hidden' id='email' value='{safeEmail}'>" +
                $"<input type='hidden' id='token' value='{safeToken}'>" +
                "<input type='password' id='pwd' placeholder='New password' " +
                "style='width:90%;padding:12px;margin:8px 0;border:1px solid #ccc;border-radius:6px;'>" +
                "<input type='password' id='pwd2' placeholder='Confirm new password' " +
                "style='width:90%;padding:12px;margin:8px 0;border:1px solid #ccc;border-radius:6px;'>" +
                "<p style='font-size:12px;color:#888;'>Min 8 chars, with upper + lower case letters and a digit.</p>" +
                "<button onclick='submitPwd()' id='btn' " +
                "style='background:#1565C0;color:#fff;border:none;padding:12px 28px;border-radius:6px;cursor:pointer;font-size:15px;'>Reset Password</button>" +
                "<p id='msg' style='font-size:14px;'></p>" +
                "</div><script>" +
                "async function submitPwd(){" +
                "var p=document.getElementById('pwd').value,q=document.getElementById('pwd2').value;" +
                "var m=document.getElementById('msg'),b=document.getElementById('btn');" +
                "if(p!==q){m.style.color='#C62828';m.textContent='Passwords do not match.';return;}" +
                "if(p.length<8){m.style.color='#C62828';m.textContent='Password must be at least 8 characters.';return;}" +
                "b.disabled=true;b.textContent='Saving...';" +
                "try{" +
                "var r=await fetch('reset-password',{method:'POST',headers:{'Content-Type':'application/json'}," +
                "body:JSON.stringify({email:document.getElementById('email').value,token:document.getElementById('token').value,newPassword:p})});" +
                "var d=await r.json();" +
                "if(r.ok){m.style.color='#2E7D32';m.textContent='Password reset successfully. You can now log in.';b.textContent='Done';}" +
                "else{var t='Error '+r.status;" +
                "if(d){if(d.errors&&d.errors.length)t=d.errors.map(function(e){return e.message;}).join(' ');" +
                "else if(d.error&&d.error.title)t=d.error.detail||d.error.title;}" +
                "m.style.color='#C62828';m.textContent=t;b.disabled=false;b.textContent='Reset Password';}" +
                "}catch(e){m.style.color='#C62828';m.textContent='Something went wrong. Try again.';b.disabled=false;b.textContent='Reset Password';}" +
                "}" +
                "</script></body></html>", "text/html");
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

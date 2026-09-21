using Elara.Application.DTOs.Auth;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Repository.Auth;
using Elara.Application.Interfaces.Service.Auth;
using Elara.Domain.Entities;
using Elara.Domain.Entities.Auth;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace Elara.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository repo;
        private readonly IEmailService emailService;
        private readonly IEmailConfirmationRepository emailConfirmationRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly ITokenService tokenService;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly IPasswordResetTokenRepository passwordResetTokenRepository;
        private readonly IRevokedTokenRepository revokedTokenRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IConfiguration configuration;
        public AuthService(
            IUserRepository repo,
            IEmailService emailService,
            IEmailConfirmationRepository emailConfirmationRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordResetTokenRepository passwordResetTokenRepository,
            IRevokedTokenRepository revokedTokenRepository,
            IRoleRepository roleRepository,
            IConfiguration configuration
        )
        {
            this.repo = repo;
            this.emailService = emailService;
            this.emailConfirmationRepository = emailConfirmationRepository;
            this.passwordHasher = passwordHasher;
            this.tokenService = tokenService;
            this.refreshTokenRepository = refreshTokenRepository;
            this.passwordResetTokenRepository = passwordResetTokenRepository;
            this.revokedTokenRepository = revokedTokenRepository;
            this.roleRepository = roleRepository;
            this.configuration = configuration;
        }

        public async Task VerifyEmailAsync(VerifyEmailRequest request)
        {
            var user = await repo.GetByEmailAsync(request.Email)
                ?? throw new KeyNotFoundException("User not found.");

            var confirmation = await emailConfirmationRepository.GetValidTokenAsync(request.Email, request.Token)
                ?? throw new ArgumentException("Invalid or expired confirmation token.");

            user.EmailConfirmed = true;
            await repo.UpdateUserAsync(user);
            await emailConfirmationRepository.MarkAsUsedAsync(confirmation);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await repo.GetByEmailAsync(request.Email)
                ?? throw new KeyNotFoundException("User not found.");

            var resetToken = GenerateToken();
            await passwordResetTokenRepository.AddAsync(new PasswordResetToken
            {
                UserId = user.Id,
                Token = resetToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsUsed = false
            });

            await emailService.SendPasswordResetAsync(user.Email, resetToken);
        }

        public async Task<AuthResponse> GoogleLoginAsync(GoogleLoginRequest request)
        {
            var payload = await VerifyGoogleTokenAsync(request.IdToken)
               ?? throw new UnauthorizedAccessException("Invalid Google token.");

            if (payload.EmailVerified != "true")    
                throw new UnauthorizedAccessException("Google email is not verified.");
            

            if (payload.Aud != configuration["Google:ClientId"])
                throw new UnauthorizedAccessException("Invalid Google token.");

            var user = await repo.GetByEmailAsync(payload.Email);

            if (user == null)
            {
                var customerRole = await roleRepository.GetByNameAsync("Customer")
                    ?? throw new InvalidOperationException("Default role 'Customer' not found.");

                user = new User
                {
                    Username = payload.Email,
                    Email = payload.Email,
                    PhoneNumber = "",
                    FullName = payload.Name,
                    PasswordHash = passwordHasher.Hash(Guid.NewGuid().ToString()),
                    EmailConfirmed = true,
                    IsActive = true,
                    UserRoles = [new UserRole { RoleId = customerRole.Id }]
                };

                await repo.AddUserAsync(user);
            }

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await repo.GetByEmailAsync(request.Email)
                ?? throw new UnauthorizedAccessException("Invalid Email or Password");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated.");

            if (!user.EmailConfirmed)
                throw new UnauthorizedAccessException("Please confirm your email first.");

            if (!passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid Email or Password");

            return await GenerateAuthResponseAsync(user);
        }

        public async Task LogoutAsync(long userId, string? jti = null)
        {
            if (!string.IsNullOrEmpty(jti))
            {
                await revokedTokenRepository.AddAsync(new RevokedToken
                {
                    Jti = jti,
                    RevokedAt = DateTime.UtcNow,
                    Reason = "Logout"
                });
            }

            var activeToken = await refreshTokenRepository.GetActiveTokenByUserIdAsync(userId);
            if (activeToken != null)
            {
                activeToken.IsRevoked = true;
                activeToken.RevokedAt = DateTime.UtcNow;
                await refreshTokenRepository.UpdateAsync(activeToken);
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var principal = tokenService.GetPrincipalFromExpiredToken(request.Token)
                ?? throw new UnauthorizedAccessException("Invalid token.");

            var userIdValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Invalid token.");

            var storedRefreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (storedRefreshToken.IsRevoked)
            {
                await revokedTokenRepository.AddAsync(new RevokedToken
                {
                    Jti = principal.FindFirst("jti")?.Value ?? "",
                    RevokedAt = DateTime.UtcNow,
                    Reason = "Refresh token reuse detected"
                });
                throw new UnauthorizedAccessException("Refresh token has been revoked.");
            }

            if (!storedRefreshToken.IsActive)
                throw new UnauthorizedAccessException("Refresh token has expired.");

            var user = await repo.GetByIdAsync(long.Parse(userIdValue))
                ?? throw new UnauthorizedAccessException("User not found.");

            var newResponse = await GenerateAuthResponseAsync(user);

            storedRefreshToken.IsRevoked = true;
            storedRefreshToken.RevokedAt = DateTime.UtcNow;
            storedRefreshToken.ReplacedByToken = newResponse.RefreshToken;
            await refreshTokenRepository.UpdateAsync(storedRefreshToken);

            return newResponse;
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            var role = await roleRepository.GetByNameAsync(request.Role.ToString())
                ?? throw new InvalidOperationException($"Role '{request.Role}' does not exist.");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber ?? "",
                FullName = request.FullName ?? request.Username,
                PasswordHash = passwordHasher.Hash(request.Password),
                EmailConfirmed = false,
                IsActive = true,
                UserRoles = [new UserRole { RoleId = role.Id }]
            };
            await repo.AddUserAsync(user);

            var emailToken = GenerateToken();
            await emailConfirmationRepository.AddAsync(new EmailConfirmation
            {
                UserId = user.Id,
                Token = emailToken,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsUsed = false
            });
            await emailService.SendEmailConfirmationAsync(user.Email, emailToken);

            return null;
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await repo.GetByEmailAsync(request.Email)
               ?? throw new KeyNotFoundException("User not found.");

            var storedToken = await passwordResetTokenRepository.GetValidTokenAsync(request.Email, request.Token)
                ?? throw new ArgumentException("Invalid or expired reset token.");

            user.PasswordHash = passwordHasher.Hash(request.NewPassword);
            await repo.UpdateUserAsync(user);
            await passwordResetTokenRepository.MarkAsUsedAsync(storedToken);
        }

        public async Task RevokeTokenAsync(string refreshToken, string? reason = null)
        {
            var token = await refreshTokenRepository.GetByTokenAsync(refreshToken)
               ?? throw new KeyNotFoundException("Refresh token not found.");

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;

            await refreshTokenRepository.UpdateAsync(token);
        }

        public async Task ResendConfirmationEmailAsync(string email)
        {
            var user = await repo.GetByEmailAsync(email)
                ?? throw new KeyNotFoundException("User not found.");

            if (user.EmailConfirmed)
                throw new InvalidOperationException("Email is already confirmed.");

            await emailConfirmationRepository.InvalidateAllAsync(user.Id);

            var emailToken = GenerateToken();
            await emailConfirmationRepository.AddAsync(new EmailConfirmation
            {
                UserId = user.Id,
                Token = emailToken,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsUsed = false
            });
            await emailService.SendEmailConfirmationAsync(user.Email, emailToken);
        }





        private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
        {
            var roles = user.UserRoles.Select(x => x.Role!.Name).ToList();
            var accessToken = tokenService.GenerateAccessToken(user.Id, user.Email, roles);
            var refreshToken = tokenService.GenerateRefreshToken();

            await refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedByIp = "system"
            });

            var durationMinutes = double.Parse(configuration["Jwt:DurationInMinutes"]!);
            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "Customer";

            return new AuthResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(durationMinutes),
                UserId = user.Id.ToString(),
                Email = user.Email,
                FullName = user.FullName,
                Role = role
            };
        }

        private static string GenerateToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        }


        private async Task<GoogleJsonPayload?> VerifyGoogleTokenAsync(string idToken)
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetFromJsonAsync<GoogleJsonPayload>(
                    $"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");
                return response;
            }
            catch
            {
                return null;
            }


        }
        public class GoogleJsonPayload
        {
            [JsonPropertyName("email")]
            public string Email { get; set; } = "";
            [JsonPropertyName("name")]
            public string Name { get; set; } = "";
            [JsonPropertyName("sub")]
            public string Sub { get; set; } = "";
            [JsonPropertyName("aud")]
            public string Aud { get; set; } = "";
            [JsonPropertyName("email_verified")]
            public string EmailVerified { get; set; } = "false";
        }
    }
}

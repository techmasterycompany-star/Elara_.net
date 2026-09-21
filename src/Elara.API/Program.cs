using DotNetEnv;
using Elara.API.Exceptions;
using Elara.Application;
using Elara.Application.Interfaces.Repository.Auth;
using Elara.Infrastructure;
using Elara.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Elara.API.Filters;
using Elara.Infrastructure.Options;

// .ENV
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Global Exception Handler
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Dependency Injection
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationActionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();

// Authorization Swagger Button
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter: Bearer {your JWT token}"
        });

    options.AddSecurityRequirement(doc =>
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer", doc),
                new List<string>()
            }
        });
});

// JWT Options
var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException(
        "JWT configuration is missing.");

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.Key) &&
        !string.IsNullOrWhiteSpace(options.Issuer) &&
        !string.IsNullOrWhiteSpace(options.Audience) &&
        options.DurationInMinutes > 0,
        "Invalid JWT configuration.")
    .ValidateOnStart();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.Key)),
                ClockSkew = TimeSpan.Zero,
            };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var jti = context.Principal?
                    .FindFirst(JwtRegisteredClaimNames.Jti)?
                    .Value;

                if (!string.IsNullOrEmpty(jti))
                {
                    var revokedRepository = context.HttpContext
                        .RequestServices
                        .GetRequiredService<
                            IRevokedTokenRepository>();

                    if (await revokedRepository.IsRevokedAsync(jti))
                    {
                        context.Fail("Token has been revoked.");
                    }
                }
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    // for (login, register, refresh)
    options.AddFixedWindowLimiter("auth-sensitive", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    // for read-only endpoints
    options.AddFixedWindowLimiter("auth-relaxed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 20;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

builder.Services
    .AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection("Email"))
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.ClientUrl) &&
        !string.IsNullOrWhiteSpace(options.Smtp.Host) &&
        options.Smtp.Port > 0 &&
        !string.IsNullOrWhiteSpace(options.Smtp.Username) &&
        !string.IsNullOrWhiteSpace(options.Smtp.Password),
        "Invalid email configuration.")
    .ValidateOnStart();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.MapControllers();

app.Run();

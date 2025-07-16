using Grpc.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TherapeutKalendar.Shared.Protos;

namespace AuthService.Services;

public class AuthServiceImpl : TherapeutKalendar.Shared.Protos.AuthService.AuthServiceBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<AuthServiceImpl> _logger;

    public AuthServiceImpl(IConfiguration config, ILogger<AuthServiceImpl> logger)
    {
        _config = config;
        _logger = logger;
    }

    public override async Task<AuthResponse> Authenticate(AuthRequest request, ServerCallContext context)
    {
        // Replace with your actual user validation
        if (!ValidateUser(request.Username, request.Password))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid credentials"));
        }

        var token = GenerateJwtToken(request.Username);
        return new AuthResponse {
            Token = token,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
        };
    }

    public override Task<TokenValidationResponse> ValidateToken(TokenRequest request, ServerCallContext context)
    {
        var principal = ValidateJwtToken(request.Token);
        return Task.FromResult(new TokenValidationResponse {
            IsValid = principal != null,
            UserId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "",
            Role = principal?.FindFirst(ClaimTypes.Role)?.Value ?? ""
        });
    }

    private bool ValidateUser(string username, string password)
    {
        // Implement your user validation logic
        return username == "admin" && password == "admin123"; // Demo only
    }

    private string GenerateJwtToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Therapist") // or "Patient"
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal? ValidateJwtToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ToDoApp.Application.Common.Interfaces;
using ToDoApp.Infrastructure.Persistence;

namespace ToDoApp.Infrastructure.Identity;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TimeSpan _accessTokenLifetime;
    private readonly TimeSpan _refreshTokenLifetime;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _secretKey;

    public JwtTokenService(IConfiguration config, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _config = config;
        _context = context;
        _userManager = userManager;
        
        _accessTokenLifetime = TimeSpan.FromMinutes(_config.GetValue<int>("Jwt:AccessMinutes", 15));
        _refreshTokenLifetime = TimeSpan.FromMinutes(_config.GetValue<int>("Jwt:RefreshMinutes", 30));
        _issuer = _config["Jwt:Issuer"]!;
        _audience = _config["Jwt:Audience"]!;
        _secretKey = _config["Jwt:Key"]!;
    }
    
    public async Task<TokenPair> CreateTokenPairAsync(IdentityUserDto dto, string? ipAddress = null)
    {
        var user = await _userManager.FindByIdAsync(dto.Id.ToString()) ?? throw new InvalidOperationException("User not found");
        
        var accessToken = CreateAccessToken(user, out var accessTokenExpiration);
        var refreshToken = CreateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.Add(_refreshTokenLifetime);

        var rt = new RefreshToken()
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = refreshTokenExpiration,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.RefreshTokens.Add(rt);
        await _context.SaveChangesAsync();
        
        return new TokenPair(accessToken, refreshToken, accessTokenExpiration, refreshTokenExpiration);
    }

    public async Task<TokenPair> RefreshAsync(string refreshToken, string? ipAddress = null)
    {
        var rt = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        if (rt is null || rt.IsRevoked || rt.ExpiresAt <= DateTime.UtcNow) return null;
        
        var user = await _userManager.FindByIdAsync(rt.UserId.ToString());
        if (user is null) return null;

        rt.IsRevoked = true;
        rt.ReplacedByToken = CreateRefreshToken();
        _context.RefreshTokens.Update(rt);
        
        var accessToken = CreateAccessToken(user, out var accessTokenExpiration);
        var refreshTokenNew = CreateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.Add(_refreshTokenLifetime);

        var rtNew = new RefreshToken()
        {
            UserId = user.Id,
            Token = refreshTokenNew,
            ExpiresAt = refreshTokenExpiration,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.RefreshTokens.Add(rtNew);
        await _context.SaveChangesAsync();
        
        return new TokenPair(accessToken, refreshTokenNew, refreshTokenExpiration, refreshTokenExpiration);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, string? ipAddress = null)
    {
        var rt = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        if (rt is null || rt.IsRevoked) return false;
        
        rt.IsRevoked = true;
        _context.RefreshTokens.Update(rt);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        var tokens = await _context.RefreshTokens.Where(r => r.UserId == userId && !r.IsRevoked).ToListAsync();
        foreach (var token in tokens) token.IsRevoked = true;
        _context.RefreshTokens.UpdateRange(tokens);
        await _context.SaveChangesAsync();
    }

    private string CreateAccessToken(ApplicationUser user, out DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? user.Email ?? ""),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        
        expires = DateTime.UtcNow.Add(_accessTokenLifetime);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string CreateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
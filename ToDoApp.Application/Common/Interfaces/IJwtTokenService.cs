namespace ToDoApp.Application.Common.Interfaces;

public record TokenPair(string AccessToken, string RefreshToken, DateTime AccessTokenExpiration, DateTime RefreshTokenExpiration);

public interface IJwtTokenService
{
    Task<TokenPair> CreateTokenPairAsync(IdentityUserDto dto, string? ipAddress = null);
    Task<TokenPair> RefreshAsync(string refreshToken, string? ipAddress = null);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken, string? ipAddress = null);
    Task RevokeAllForUserAsync(Guid userId);
}
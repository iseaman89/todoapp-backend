using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ToDoApp.Application.Common.Interfaces;

namespace ToDoApp.Infrastructure.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _contextAccessor;

    public UserContextService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var id = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? _contextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return id is not null ? Guid.Parse(id) : null;
        }
    }

    public string? UserName => 
        _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)
        ?? _contextAccessor.HttpContext?.User?.Identity?.Name;
}
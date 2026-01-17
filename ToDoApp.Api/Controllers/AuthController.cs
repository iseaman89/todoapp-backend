using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using ToDoApp.Api.Models;
using ToDoApp.Application.Common.Interfaces;
using ToDoApp.Infrastructure.Identity;
using ToDoApp.Infrastructure.Persistence;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _tokenService;
    private readonly ApplicationDbContext _context;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenService tokenService, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var exists = await _userManager.FindByEmailAsync(req.Email);
        if (exists != null) return BadRequest("Email already in use");

        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = req.Email, UserName = req.Email };
        var create = await _userManager.CreateAsync(user, req.Password);
        if (!create.Succeeded) return BadRequest(create.Errors);

        var dto = new IdentityUserDto(user.Id, user.UserName ?? user.Email ?? "", user.Email ?? "");
        var pair = await _tokenService.CreateTokenPairAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString());

        return Ok(new { user = dto, tokens = pair });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null) return BadRequest("Invalid credentials");

        var check = await _signInManager.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure: false);
        if (!check.Succeeded) return BadRequest("Invalid credentials");

        var dto = new IdentityUserDto(user.Id, user.UserName ?? user.Email ?? "", user.Email ?? "");
        var pair = await _tokenService.CreateTokenPairAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString());

        return Ok(new { user = dto, tokens = pair });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        var pair = await _tokenService.RefreshAsync(req.RefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString());
        if (pair == null) return Unauthorized();

        return Ok(new { tokens = pair });
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RevokeRequest req)
    {
        var ok = await _tokenService.RevokeRefreshTokenAsync(req.RefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString());
        return ok ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpPost("revoke-all")]
    public async Task<IActionResult> RevokeAll()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userIdClaim == null) return Unauthorized();
        var userId = Guid.Parse(userIdClaim);
        await _tokenService.RevokeAllForUserAsync(userId);
        return NoContent();
    }
}
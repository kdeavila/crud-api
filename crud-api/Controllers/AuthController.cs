using crud_api.Common;
using crud_api.DTOs.Auth;
using crud_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace crud_api.Controllers;

using crud_api.Interfaces;

[Route("/api/[controller]")]
[ApiController]
[EnableRateLimiting("sliding")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        
        var result = await authService.Login(userLoginDto);

        return (result.Status) switch
        {
            ServiceResultStatus.Success => Ok(new { result.JWT }),
            ServiceResultStatus.InvalidInput => BadRequest(result.Message),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            _ => StatusCode(500, "There was an error logging in.")
        };
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.Register(userRegisterDto);

        return (result.Status) switch
        {
            ServiceResultStatus.Success => Ok(),
            ServiceResultStatus.InvalidInput => BadRequest(result.Message),
            ServiceResultStatus.Conflict => Conflict(result.Message),
            _ => StatusCode(500, "There was an error registering.")
        };
    }
}
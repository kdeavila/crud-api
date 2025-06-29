using crud_api.Common;
using crud_api.DTOs;
using crud_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class AuthController(AuthService authService) : ControllerBase
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
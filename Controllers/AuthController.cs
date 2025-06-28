using crud_api.DTOS;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromBody] UserLoginDto userLoginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok("Login successful");
    }
}
using crud_api.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Controllers;

[Route("/[controller]")]
[ApiController]
public class ProfileController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet("list")]
    public async Task<IActionResult> Get()
    {
        var profileList = await _context.Profiles.ToListAsync();
        return Ok(profileList);
    }
}
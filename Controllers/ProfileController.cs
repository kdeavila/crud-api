using crud_api.Context;
using crud_api.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Controllers;

[Route("/[controller]")]
[ApiController]
public class ProfileController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet("list")]
    public async Task<ActionResult<List<ProfileDTO>>> Get()
    {
        var dtoList = new List<ProfileDTO>();
        var dbList = await _context.Profiles.ToListAsync();

        foreach (var p in dbList)
        {
            dtoList.Add(new ProfileDTO
            {
                Id = p.Id,
                Name = p.Name,
            });
        }
        
        return Ok(dtoList);
    }
}
using crud_api.DTOS;
using crud_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class ProfileController(ProfileService profileService) : ControllerBase
{
    private readonly ProfileService _profileService = profileService;

    [HttpGet("getall")]
    public async Task<ActionResult<List<ProfileDto>>> GetAll()
    {
        var profiles = await _profileService.List();
        return Ok(profiles);
    }
}
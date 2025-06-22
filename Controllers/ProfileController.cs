using crud_api.Context;
using crud_api.DTOS;
using crud_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Controllers;

[Route("/[controller]")]
[ApiController]
public class ProfileController(ProfileService profileService) : ControllerBase
{
    private readonly ProfileService _profileService = profileService;

    [HttpGet("getall")]
    public async Task<ActionResult<List<ProfileDTO>>> GetAll()
    {
        var profiles = await _profileService.List();
        return Ok(profiles);
    }
}
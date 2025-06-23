using crud_api.Common;
using crud_api.DTOs;
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

    [HttpGet("getbyid/{id:int}")]
    public async Task<ActionResult<ProfileDto>> GetById(int id)
    {
        var profile = await _profileService.GetById(id);
        if (profile is null) return NotFound("Profile not found!");
        return Ok(profile);
    }

    [HttpPost("create")]
    public async Task<ActionResult<ProfileDto>> Create(ProfileDto dtoProfile)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _profileService.Create(dtoProfile);
        return result.Status switch
        {
            ServiceResultStatus.Success => CreatedAtAction(nameof(GetById),
                new { id = result.Data!.Id },
                result.Data),
            ServiceResultStatus.Conflict => Conflict(result.Message),
            _ => StatusCode(500, "There was an error creating the profile.")
        };
    }

    [HttpPut("update/{id:int}")]
    public async Task<ActionResult<ProfileDto>> Update(int id, [FromBody] ProfileDto dtoProfile)
    {
        if (id != dtoProfile.Id) return BadRequest("Id does not match with Id in body data.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _profileService.Update(id, dtoProfile);
        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Data),
            ServiceResultStatus.Conflict => Conflict(result.Message),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            _ => StatusCode(500, "There was an error updating the profile.")
        };
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _profileService.Delete(id);
        if (!result) return NotFound("Profile not found!");
        return NoContent();
    }
}
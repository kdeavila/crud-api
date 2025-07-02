using crud_api.Common;
using crud_api.DTOs.Profile;
using crud_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class ProfileController(ProfileService profileService) : ControllerBase
{
    private readonly ProfileService _profileService = profileService;

    [HttpGet("getall")]
    [Authorize(Roles = "Admin, Editor, Viewer")]
    public async Task<ActionResult<List<ProfileGetDto>>> GetAll([FromQuery] ProfileQueryParams profileQueryParams)
    {
        var result = await _profileService.GetAllPaginatedAndFiltered(profileQueryParams);

        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Data),
            ServiceResultStatus.InvalidInput => BadRequest(result.Message),
            _ => StatusCode(500, "There was an error getting all profiles.")
        };
    }

    [HttpGet("getbyid/{id:int}")]
    [Authorize(Roles = "Admin, Editor, Viewer")]
    public async Task<ActionResult<ProfileGetDto>> GetById(int id)
    {
        var result = await _profileService.GetById(id);
        
        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Data),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            _ => StatusCode(500, "There was an error getting the profile.")
        };
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<ActionResult<ProfileGetDto>> Create(ProfileCreateDto profileCreateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _profileService.Create(profileCreateDto);
        return result.Status switch
        {
            ServiceResultStatus.Success => CreatedAtAction(nameof(GetById),
                new { id = result.Data!.Id },
                result.Data),
            ServiceResultStatus.Conflict => Conflict(result.Message),
            _ => StatusCode(500, "There was an error creating the profile.")
        };
    }

    [Authorize(Roles = "Admin, Editor")]
    [HttpPut("update/{id:int}")]
    public async Task<ActionResult<ProfileGetDto>> Update(int id, [FromBody] ProfileUpdateDto profileUpdateDto)
    {
        if (id != profileUpdateDto.Id) return BadRequest("Id does not match with Id in body data.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _profileService.Update(id, profileUpdateDto);
        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Data),
            ServiceResultStatus.Conflict => Conflict(result.Message),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            _ => StatusCode(500, "There was an error updating the profile.")
        };
    }

    [Authorize(Roles = "Admin, Editor")]
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _profileService.Delete(id);

        return result.Status switch
        {
            ServiceResultStatus.Success => NoContent(),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            _ => StatusCode(500, "There was an error deleting the profile.")
        };
    }
}
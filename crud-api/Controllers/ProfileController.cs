using crud_api.Common;
using crud_api.DTOs.Profile;
using crud_api.Interfaces;
using crud_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace crud_api.Controllers;

[Route("/api/[controller]")]
[ApiController]
[EnableRateLimiting("sliding")]
public class ProfileController(IProfileService profileService) : BaseController
{
    private readonly IProfileService _profileService = profileService;

    [HttpGet("getall")]
    [Authorize(Roles = "Admin, Editor, Viewer")]
    public async Task<IActionResult> GetAll([FromQuery] ProfileQueryParams profileQueryParams)
    {
        var serviceResult = await _profileService.GetAllPaginatedAndFiltered(profileQueryParams);
        return ProcessServiceResult(serviceResult);
    }

    [HttpGet("getbyid/{id:int}")]
    [Authorize(Roles = "Admin, Editor, Viewer")]
    public async Task<IActionResult> GetById(int id)
    {
        var serviceResult = await _profileService.GetById(id);
        return ProcessServiceResult(serviceResult);
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Create(ProfileCreateDto profileCreateDto)
    {
        var serviceResult = await _profileService.Create(profileCreateDto);

        return serviceResult.Status == ServiceResultStatus.Created
            ? ProcessServiceResultForCreate(serviceResult, nameof(GetById), new { id = serviceResult.Data!.Id })
            : ProcessServiceResult(serviceResult);
    }

    [Authorize(Roles = "Admin, Editor")]
    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProfileUpdateDto profileUpdateDto)
    {
        var serviceResult = await _profileService.Update(id, profileUpdateDto);
        return ProcessServiceResult(serviceResult);
    }

    [Authorize(Roles = "Admin, Editor")]
    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var serviceResult = await _profileService.Delete(id);
        return ProcessServiceResult(serviceResult);
    }
}
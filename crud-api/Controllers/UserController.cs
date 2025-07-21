using crud_api.DTOs.User;
using crud_api.Interfaces;
using crud_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : BaseController
{
    private readonly IUserService _userService = userService;

    [Authorize(Roles = "Admin")]
    [HttpGet("getall")]
    public async Task<IActionResult> GetAll([FromQuery] UserQueryParamsDto userQueryParamsDto)
    {
        var serviceResult = await _userService.GetAllPaginatedAndFiltered(userQueryParamsDto);
        return ProcessServiceResult(serviceResult);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("getbyid/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var serviceResult = await _userService.GetById(id);
        return ProcessServiceResult(serviceResult);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto userUpdateDto)
    {
        var serviceResult = await _userService.Update(id, userUpdateDto);
        return ProcessServiceResult(serviceResult);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var serviceResult = await _userService.Delete(id);
        return ProcessServiceResult(serviceResult);
    }
}
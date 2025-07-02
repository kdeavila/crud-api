using crud_api.Common;
using crud_api.DTOs.User;
using crud_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class UserController(UserService userService) : ControllerBase
{
    private readonly UserService _userService = userService;

    [Authorize(Roles = "Admin")]
    [HttpGet("getall")]
    public async Task<ActionResult<List<UserGetDto>>> GetAll([FromQuery] UserQueryParamsDto userQueryParamsDto)
    {
        var result = await _userService.GetAllPaginatedAndFiltered(userQueryParamsDto);

        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Data),
            ServiceResultStatus.InvalidInput => BadRequest(result.Message),
            _ => StatusCode(500, "There was an error getting all users.")
        };
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("getbyid/{id:int}")]
    public async Task<ActionResult<UserGetDto>> GetById(int id)
    {
        var result = await _userService.GetById(id);

        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Data),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            _ => StatusCode(500, "There was an error getting the user.")
        };
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update/{id:int}")]
    public async Task<ActionResult<UserGetDto>> Update(int id, [FromBody] UserUpdateDto userUpdateDto)
    {
        if (id != userUpdateDto.Id) return BadRequest("Id does not match with Id in body data.");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var result = await _userService.Update(id, userUpdateDto);

        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Data),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            ServiceResultStatus.Conflict => Conflict(result.Message),
            _ => StatusCode(500, "There was an error updating the user.")
        };
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _userService.Delete(id);
        
        return result.Status switch
        {
            ServiceResultStatus.Success => NoContent(),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            _ => StatusCode(500, "There was an error deleting the user.")
        };
    }
}
using crud_api.Common;
using crud_api.DTOs.Employee;
using crud_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class EmployeeController(EmployeeService employeeService) : ControllerBase
{
    private readonly EmployeeService _employeeService = employeeService;

    [HttpGet("getall")]
    [Authorize(Roles = "Admin, Editor, Viewer")]
    public async Task<ActionResult<List<EmployeeGetDto>>> GetAll(
        [FromQuery] EmployeeQueryParamsDto employeeQueryParamsDto)
    {
        var result = await _employeeService.GetAllPaginatedAndFiltered(employeeQueryParamsDto);

        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Data),
            ServiceResultStatus.InvalidInput => BadRequest(result.Message),
            _ => StatusCode(500, "There was an error getting all employees.")
        };
    }

    [Authorize(Roles = "Admin, Editor, Viewer")]
    [HttpGet("getbyid/{id:int}")]
    public async Task<ActionResult<EmployeeGetDto>> GetById(int id)
    {
        var result = await _employeeService.GetById(id);

        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Data),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            _ => StatusCode(500, "There was an error getting the employee.")
        };
    }

    [Authorize(Roles = "Admin, Editor")]
    [HttpPost("create")]
    public async Task<ActionResult<EmployeeGetDto>> Create(EmployeeCreateDto employeeCreateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _employeeService.Create(employeeCreateDto);

        return result.Status switch
        {
            ServiceResultStatus.Success => CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data),
            ServiceResultStatus.InvalidInput => BadRequest(result.Message),
            _ => StatusCode(500, "There was an error creating the employee.")
        };
    }

    [Authorize(Roles = "Admin, Editor")]
    [HttpPut("update/{id:int}")]
    public async Task<ActionResult<EmployeeGetDto>> Update(int id, [FromBody] EmployeeUpdateDto employeeUpdateDto)
    {
        if (id != employeeUpdateDto.Id) return BadRequest("Id does not match with Id in body data.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _employeeService.Update(id, employeeUpdateDto);

        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Data),
            ServiceResultStatus.InvalidInput => BadRequest(result.Message),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            _ => StatusCode(500, "There was an error updating the employee.")
        };
    }

    [Authorize(Roles = "Admin, Editor")]
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _employeeService.Delete(id);

        return result.Status switch
        {
            ServiceResultStatus.Success => NoContent(),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            _ => StatusCode(500, "There was an error deleting the employee.")
        };
    }
}
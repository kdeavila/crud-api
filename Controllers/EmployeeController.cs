using crud_api.Common;
using crud_api.DTOS;
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
    public async Task<ActionResult<List<EmployeeDto>>> GetAll()
    {
        var employees = await _employeeService.GetAll();
        return Ok(employees);
    }

    [HttpGet("getbyid/{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        var employee = await _employeeService.GetById(id);

        if (employee is null) return NotFound("Employee not found!");
        return Ok(employee);
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<EmployeeDto>> Create(EmployeeDto dtoEmployee)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _employeeService.Create(dtoEmployee);

        return result.Status switch
        {
            ServiceResultStatus.Success => CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data),
            ServiceResultStatus.InvalidInput => BadRequest(result.Message),
            _ => StatusCode(500, "There was an error creating the employee.")
        };
    }

    [Authorize]
    [HttpPut("update/{id:int}")]
    public async Task<ActionResult<EmployeeDto>> Update(int id, [FromBody] EmployeeDto dtoEmployee)
    {
        if (id != dtoEmployee.Id) return BadRequest("Id does not match with Id in body data.");

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _employeeService.Update(id, dtoEmployee);

        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Data),
            ServiceResultStatus.InvalidInput => BadRequest(result.Message),
            ServiceResultStatus.NotFound => NotFound(result.Message),
            _ => StatusCode(500, "There was an error updating the employee.")
        };
    }

    [Authorize]
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var employee = await _employeeService.Delete(id);
        if (!employee) return NotFound($"Employee with id {id} not found!");
        return NoContent();
    }
}
using crud_api.DTOS;
using crud_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("/[controller]")]
[ApiController]
public class EmployeeController(EmployeeService employeeService) : ControllerBase
{
    private readonly EmployeeService _employeeService = employeeService;

    [HttpGet("getall")]
    public async Task<ActionResult<List<EmployeeDTO>>> GetAll()
    {
        var employees = await _employeeService.GetAll();
        return Ok(employees);
    }

    [HttpGet("getbyid/{id:int}")]
    public async Task<ActionResult<EmployeeDTO>> GetById(int id)
    {
        var employee = await _employeeService.GetById(id);
        
        if (employee is null) return NotFound("Employee not found!");
        return Ok(employee);
    }

    [HttpPost("create")]
    public async Task<ActionResult<EmployeeDTO>> Create(EmployeeDTO dtoEmployee)
    {
        var newEmployee = await _employeeService.Create(dtoEmployee);

        if (newEmployee is null)
        {
            return BadRequest("Employee not created!");
        }
        
        return Ok(newEmployee);
    }

    [HttpPut("update/{id:int}")]
    public async Task<ActionResult<EmployeeDTO>> Update(int id, [FromBody] EmployeeDTO dtoEmployee)
    {
        if (id != dtoEmployee.Id) return BadRequest("Id does not match!");
        
        var employee = await _employeeService.Update(id, dtoEmployee);
        if (employee is null) return NotFound("Employee not found!");
        return Ok(employee);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult<EmployeeDTO>> Delete(int id)
    {
        var employee = await _employeeService.Delete(id);
        if (!employee) return NotFound("Employee not found!");
        return Ok();
    }
}
using crud_api.Common;
using crud_api.DTOs.Employee;
using crud_api.Interfaces;
using crud_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class EmployeeController(IEmployeeService employeeService) : BaseController
{
    private readonly IEmployeeService  _employeeService = employeeService;

    [HttpGet("getall")]
    [Authorize(Roles = "Admin, Editor, Viewer")]
    public async Task<IActionResult> GetAll(
        [FromQuery] EmployeeQueryParamsDto employeeQueryParamsDto)
    {
        var serviceResult = await _employeeService.GetAllPaginatedAndFiltered(employeeQueryParamsDto);
        return ProcessServiceResult(serviceResult);
    }

    [Authorize(Roles = "Admin, Editor, Viewer")]
    [HttpGet("getbyid/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var serviceResult = await _employeeService.GetById(id);
        return ProcessServiceResult(serviceResult);
    }
    
    [Authorize(Roles = "Admin, Editor")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(EmployeeCreateDto employeeCreateDto)
    {
        var serviceResult = await _employeeService.Create(employeeCreateDto);

        return serviceResult.Status == ServiceResultStatus.Created
            ? ProcessServiceResultForCreate(serviceResult, nameof(GetById),
                new { id = serviceResult.Data!.Id }
            )
            : ProcessServiceResult(serviceResult);
    }

    [Authorize(Roles = "Admin, Editor")]
    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto employeeUpdateDto)
    {
        var serviceResult = await _employeeService.Update(id, employeeUpdateDto);
        return ProcessServiceResult(serviceResult);
    }

    [Authorize(Roles = "Admin, Editor")]
    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var serviceResult = await _employeeService.Delete(id);
        return ProcessServiceResult(serviceResult);
    }
}
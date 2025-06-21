using crud_api.Context;
using crud_api.DTOS;
using crud_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Controllers;

[Route("/[controller]")]
[ApiController]
public class EmployeeController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet("list")]
    public async Task<ActionResult<List<EmployeeDTO>>> Get()
    {
        var dtoList = new List<EmployeeDTO>();
        var dbList = await _context.Employees.Include(p => p.ProfileReference).ToListAsync();

        foreach (var e in dbList)
        {
            dtoList.Add(new EmployeeDTO
            {
                Id = e.Id,
                FullName = e.FullName,
                Salary = e.Salary,
                IdProfile = e.IdProfile,
                NameProfile = e.ProfileReference.Name
            });
        }

        return Ok(dtoList);
    }

    [HttpGet("list/{id}")]
    public async Task<ActionResult<EmployeeDTO>> Get(int id)
    {
        var dtoEmployee = new EmployeeDTO();
        var dbEmployee = await _context.Employees.Include(p => p.ProfileReference).Where(e => e.Id == id).FirstAsync();

        dtoEmployee.Id = id;
        dtoEmployee.FullName = dbEmployee.FullName;
        dtoEmployee.Salary = dbEmployee.Salary;
        dtoEmployee.IdProfile = dbEmployee.IdProfile;
        dtoEmployee.NameProfile = dbEmployee.ProfileReference.Name;
        return Ok(dtoEmployee);
    }

    [HttpPost("create")]
    public async Task<ActionResult<EmployeeDTO>> Create(EmployeeDTO dtoEmployee)
    {
        var dbEmployee = new Employee
        {
            FullName = dtoEmployee.FullName,
            Salary = dtoEmployee.Salary,
            IdProfile = dtoEmployee.IdProfile,
        };

        await _context.Employees.AddAsync(dbEmployee);
        await _context.SaveChangesAsync();

        var createdEmployeeDto = await _context.Employees
            .Include(e => e.ProfileReference)
            .Where(e => e.Id == dbEmployee.Id)
            .Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FullName = e.FullName,
                Salary = e.Salary,
                IdProfile = e.IdProfile,
                NameProfile = e.ProfileReference.Name
            }).FirstAsync();
        
        return Ok(createdEmployeeDto);
    }

    [HttpPut("edit")]
    public async Task<ActionResult<EmployeeDTO>> Edit(EmployeeDTO dtoEmployee)
    {
        var dbEmployee = await _context.Employees.Include(p => p.ProfileReference)
            .Where(e => e.Id == dtoEmployee.Id).FirstAsync();

        dbEmployee.FullName = dtoEmployee.FullName;
        dbEmployee.Salary = dtoEmployee.Salary;
        dbEmployee.IdProfile = dtoEmployee.IdProfile;

        _context.Employees.Update(dbEmployee);
        await _context.SaveChangesAsync();
        return Ok("Employee edited successfully!");
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<EmployeeDTO>> Delete(int id)
    {
        var dbEmployee = await _context.Employees.Where(e => e.Id == id).FirstAsync();

        if (dbEmployee is null)
        {
            return NotFound("Employee not found!");
        }

        _context.Employees.Remove(dbEmployee);
        await _context.SaveChangesAsync();
        
        return Ok("Employee deleted successfully!");
    }
}
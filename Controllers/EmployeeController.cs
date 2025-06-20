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
            IdProfile = dtoEmployee.IdProfile
        };
        
        await _context.Employees.AddAsync(dbEmployee);
        await _context.SaveChangesAsync();
        return Ok(dbEmployee);
    }
}
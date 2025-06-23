using crud_api.Common;
using crud_api.Context;
using crud_api.DTOS;
using crud_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class EmployeeService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<EmployeeDto>> GetAll()
    {
        var dtoList = await _context.Employees
            .Include(p => p.ProfileReference)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Salary = e.Salary,
                IdProfile = e.IdProfile,
                NameProfile = e.ProfileReference!.Name,
            })
            .ToListAsync();

        return dtoList;
    }

    public async Task<EmployeeDto?> GetById(int id)
    {
        var dtoEmployee = await _context.Employees
            .Include(p => p.ProfileReference)
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Salary = e.Salary,
                IdProfile = e.IdProfile,
                NameProfile = e.ProfileReference!.Name,
            })
            .FirstOrDefaultAsync();

        return dtoEmployee;
    }

    public async Task<EmployeeServiceResult> Create(EmployeeDto dtoEmployee)
    {
        var profileExists = await _context.Profiles.AnyAsync(p => p.Id == dtoEmployee.IdProfile);

        if (!profileExists)
        {
            return new EmployeeServiceResult
            {
                Status = ServiceResultStatus.InvalidInput,
                Message = $"Profile with id {dtoEmployee.IdProfile} does not exist"
            };
        }

        var dbEmployee = new Employee
        {
            FullName = dtoEmployee.FullName,
            Salary = dtoEmployee.Salary,
            IdProfile = dtoEmployee.IdProfile
        };

        await _context.Employees.AddAsync(dbEmployee);
        await _context.SaveChangesAsync();

        var createdEmployeeDto = await GetById(dbEmployee.Id);
        if (createdEmployeeDto is null)
        {
            return new EmployeeServiceResult
            {
                Status = ServiceResultStatus.InvalidInput,
                Message = "Employee not created"
            };
        }

        return new EmployeeServiceResult
        {
            Status = ServiceResultStatus.Success,
            Data = createdEmployeeDto
        };
    }

    public async Task<EmployeeServiceResult> Update(int id, EmployeeDto dtoEmployee)
    {
        var profileExists = await _context.Profiles.AnyAsync(p => p.Id == dtoEmployee.IdProfile);

        if (!profileExists)
        {
            return new EmployeeServiceResult
            {
                Status = ServiceResultStatus.InvalidInput,
                Message = $"Profile with id {dtoEmployee.IdProfile} does not exist"
            };
        }

        var dbEmployee = await _context.Employees
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();

        if (dbEmployee is null)
        {
            return new EmployeeServiceResult
            {
                Status = ServiceResultStatus.NotFound,
                Message = $"Employee with id {id} not found"
            };
        }

        dbEmployee.FullName = dtoEmployee.FullName;
        dbEmployee.Salary = dtoEmployee.Salary;
        dbEmployee.IdProfile = dtoEmployee.IdProfile;

        await _context.SaveChangesAsync();
        
        var updatedEmployeeDto = await GetById(dbEmployee.Id);
        if (updatedEmployeeDto is null)
        {
            return new EmployeeServiceResult
            {
                Status = ServiceResultStatus.InvalidInput,
                Message = "Employee not updated"
            };
        }

        return new EmployeeServiceResult
        {
            Status = ServiceResultStatus.Success,
            Data = updatedEmployeeDto
        };
    }

    public async Task<bool> Delete(int id)
    {
        var dbEmployee = await _context.Employees
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();

        if (dbEmployee is null) return false;

        _context.Employees.Remove(dbEmployee);
        await _context.SaveChangesAsync();

        return true;
    }
}
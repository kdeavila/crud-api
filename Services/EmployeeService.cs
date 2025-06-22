using crud_api.Context;
using crud_api.DTOS;
using crud_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class EmployeeService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<EmployeeDTO>> GetAll()
    {
        var dtoList = await _context.Employees
            .Include(p => p.ProfileReference)
            .Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FullName = e.FullName,
                Salary = e.Salary,
                IdProfile = e.IdProfile,
                NameProfile = e.ProfileReference.Name,
            })
            .ToListAsync();

        return dtoList;
    }

    public async Task<EmployeeDTO?> GetById(int id)
    {
        var dtoEmployee = await _context.Employees
            .Include(p => p.ProfileReference)
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FullName = e.FullName,
                Salary = e.Salary,
                IdProfile = e.IdProfile,
                NameProfile = e.ProfileReference.Name,
            })
            .FirstOrDefaultAsync();

        return dtoEmployee;
    }

    public async Task<EmployeeDTO?> Create(EmployeeDTO dtoEmployee)
    {
        if (string.IsNullOrEmpty(dtoEmployee.FullName)) return null;
        if (dtoEmployee.Salary < 0) return null;
        if (dtoEmployee.IdProfile < 0) return null;

        var dbEmployee = new Employee
        {
            FullName = dtoEmployee.FullName,
            Salary = dtoEmployee.Salary,
            IdProfile = dtoEmployee.IdProfile,
        };
        
        await _context.Employees.AddAsync(dbEmployee);
        await _context.SaveChangesAsync();

        var createdEmployeeDto = await GetById(dbEmployee.Id);
        return createdEmployeeDto;
    }

    public async Task<EmployeeDTO?> Update(int id, EmployeeDTO dtoEmployee)
    {
        if (string.IsNullOrEmpty(dtoEmployee.FullName)) return null;
        if (dtoEmployee.Salary < 0) return null;
        if (dtoEmployee.IdProfile < 0) return null;

        var dbEmployee = await _context.Employees
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();
        
        if (dbEmployee is null) return null;
        
        dbEmployee.FullName = dtoEmployee.FullName;
        dbEmployee.Salary = dtoEmployee.Salary;
        dbEmployee.IdProfile = dtoEmployee.IdProfile;
        
        await _context.SaveChangesAsync();
        return await GetById(id);
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
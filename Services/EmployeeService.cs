using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs;
using crud_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class EmployeeService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<PaginatedResultDto<EmployeeDto>> GetAllPaginatedAndFiltered(
        EmployeeQueryParamsDto employeeQueryParamsDto)
    {
        var query = _context.Employees.AsQueryable();
        query = query.Include(e => e.ProfileReference);

        if (!string.IsNullOrEmpty(employeeQueryParamsDto.FullName))
        {
            query = query.Where(e => e.FullName.Contains(employeeQueryParamsDto.FullName));
        }

        if (employeeQueryParamsDto.IdProfile.HasValue)
        {
            query = query.Where(e => e.IdProfile == employeeQueryParamsDto.IdProfile);
        }

        if (employeeQueryParamsDto.MinSalary.HasValue)
        {
            query = query.Where(e => e.Salary >= employeeQueryParamsDto.MinSalary);
        }

        if (employeeQueryParamsDto.MaxSalary.HasValue)
        {
            query = query.Where(e => e.Salary <= employeeQueryParamsDto.MaxSalary);
        }

        var sortByLower = employeeQueryParamsDto.SortBy?.ToLower();
        var orderLower = employeeQueryParamsDto.Order?.ToLower();

        switch (sortByLower)
        {
            case "fullname":
                query = (orderLower == "desc")
                    ? query.OrderByDescending(e => e.FullName)
                    : query.OrderBy(e => e.FullName);
                break;
            case "salary":
                query = (orderLower == "desc")
                    ? query.OrderByDescending(e => e.Salary)
                    : query.OrderBy(e => e.Salary);
                break;
            case "profile":
                query = (orderLower == "desc")
                    ? query.OrderByDescending(e => e.ProfileReference!.Name)
                    : query.OrderBy(e => e.ProfileReference!.Name);
                break;
            case "id":
            default:
                query = (orderLower == "desc")
                    ? query.OrderByDescending(e => e.Id)
                    : query.OrderBy(e => e.Id);
                break;
        }

        int totalCount = await query.CountAsync();

        query = query
            .Skip((employeeQueryParamsDto.PageNumber - 1) * employeeQueryParamsDto.PageSize)
            .Take(employeeQueryParamsDto.PageSize);

        var paginatedEmployees = await query
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Salary = e.Salary,
                IdProfile = e.IdProfile,
                NameProfile = e.ProfileReference!.Name
            })
            .ToListAsync();

        var result = new PaginatedResultDto<EmployeeDto>
        {
            Data = paginatedEmployees,
            TotalCount = totalCount,
            PageNumber = employeeQueryParamsDto.PageNumber,
            PageSize = employeeQueryParamsDto.PageSize
        };

        return result;
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
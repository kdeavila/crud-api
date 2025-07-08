using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs.Common;
using crud_api.DTOs.Employee;
using crud_api.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace crud_api.Services;

public class EmployeeService(AppDbContext context, ILogger<EmployeeService> logger)
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<EmployeeService> _logger = logger;

    public async Task<ServiceResult<PaginatedResultDto<EmployeeGetDto>>> GetAllPaginatedAndFiltered(
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

        var sortByLower = employeeQueryParamsDto.QueryParams.SortBy?.ToLower();
        var orderLower = employeeQueryParamsDto.QueryParams.Order?.ToLower();

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

        var totalCount = await query.CountAsync();

        query = query
            .Skip((employeeQueryParamsDto.QueryParams.PageNumber - 1) * employeeQueryParamsDto.QueryParams.PageSize)
            .Take(employeeQueryParamsDto.QueryParams.PageSize);

        var paginatedEmployees = await query
            .Select(e => new EmployeeGetDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Salary = e.Salary,
                IdProfile = e.IdProfile,
                NameProfile = e.ProfileReference!.Name
            })
            .ToListAsync();

        var paginatedResult = new PaginatedResultDto<EmployeeGetDto>
        {
            Data = paginatedEmployees,
            TotalCount = totalCount,
            PageNumber = employeeQueryParamsDto.QueryParams.PageNumber,
            PageSize = employeeQueryParamsDto.QueryParams.PageSize
        };

        var result = new ServiceResult<PaginatedResultDto<EmployeeGetDto>>
        {
            Data = paginatedResult,
            Status = ServiceResultStatus.Success
        };

        return result;
    }

    public async Task<ServiceResult<EmployeeGetDto>> GetById(int id)
    {
        var employeeDto = await _context.Employees
            .Include(p => p.ProfileReference)
            .Where(e => e.Id == id)
            .Select(e => new EmployeeGetDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Salary = e.Salary,
                IdProfile = e.IdProfile,
                NameProfile = e.ProfileReference!.Name,
            })
            .FirstOrDefaultAsync();

        if (employeeDto is null)
        {
            return new ServiceResult<EmployeeGetDto>
            {
                Status = ServiceResultStatus.NotFound,
                Message = $"Employee with id {id} not found"
            };
        }

        return new ServiceResult<EmployeeGetDto>
        {
            Status = ServiceResultStatus.Success,
            Data = employeeDto
        };
    }

    public async Task<ServiceResult<EmployeeGetDto>> Create(EmployeeCreateDto employeeCreateDto)
    {
        try
        {
            var profileExists = await _context.Profiles.AnyAsync(p => p.Id == employeeCreateDto.IdProfile);
            if (!profileExists)
            {
                return new ServiceResult<EmployeeGetDto>
                {
                    Status = ServiceResultStatus.InvalidInput,
                    Message = $"Profile with id {employeeCreateDto.IdProfile} does not exist"
                };
            }

            var dbEmployee = new Employee
            {
                FullName = employeeCreateDto.FullName,
                Salary = employeeCreateDto.Salary,
                IdProfile = employeeCreateDto.IdProfile
            };

            await _context.Employees.AddAsync(dbEmployee);
            await _context.SaveChangesAsync();

            var createdEmployeeDto = new EmployeeGetDto
            {
                Id = dbEmployee.Id,
                FullName = dbEmployee.FullName,
                Salary = dbEmployee.Salary,
                IdProfile = dbEmployee.IdProfile,
                NameProfile = await _context.Profiles
                    .Where(p => p.Id == dbEmployee.IdProfile)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync()
            };

            return new ServiceResult<EmployeeGetDto>
            {
                Status = ServiceResultStatus.Success,
                Data = createdEmployeeDto
            };
        }
        catch (DbUpdateException dbEx)
        {
            var sqlEx = dbEx.InnerException as SqlException;

            if (sqlEx != null)
            {
                if (sqlEx.Number is 2627 or 2601)
                {
                    _logger.LogWarning(dbEx,
                        "DbUpdateException: Unique constraint violation during employee creation. Details: {SqlErrorMessage}",
                        sqlEx.Message);
                    return new ServiceResult<EmployeeGetDto>
                    {
                        Status = ServiceResultStatus.Conflict,
                        Message =
                            "An employee with the same unique identifier (e.g., name if configured) already exists."
                    };
                }
                else if (sqlEx.Number is 547)
                {
                    _logger.LogWarning(dbEx,
                        "DbUpdateException: Foreign key violation during employee creation. Details: {SqlErrorMessage}",
                        sqlEx.Message);
                    return new ServiceResult<EmployeeGetDto>
                    {
                        Status = ServiceResultStatus.InvalidInput,
                        Message = $"The provided profile ID ({employeeCreateDto.IdProfile}) does not exist."
                    };
                }
                else
                {
                    _logger.LogError(dbEx,
                        "DbUpdateException: An unhandled SQL error occurred during employee creation. SQL Error Number: {SqlErrorNumber}, Message: {SqlErrorMessage}",
                        sqlEx.Number, sqlEx.Message);
                    return new ServiceResult<EmployeeGetDto>
                    {
                        Status = ServiceResultStatus.Error,
                        Message = "An unexpected database error occurred. Please try again later."
                    };
                }
            }
            else
            {
                _logger.LogError(dbEx,
                    "DbUpdateException: An unexpected database update error occurred during employee creation. Message: {DbUpdateMessage}",
                    dbEx.Message);
                return new ServiceResult<EmployeeGetDto>
                {
                    Status = ServiceResultStatus.Error,
                    Message = "An unexpected database error occurred. Please try again later."
                };
            }
        }
    }

    public async Task<ServiceResult<EmployeeGetDto>> Update(int id, EmployeeUpdateDto employeeUpdateDto)
    {
        if (employeeUpdateDto.IdProfile.HasValue)
        {
            if (id != employeeUpdateDto.Id)
                return new ServiceResult<EmployeeGetDto>
                {
                    Status = ServiceResultStatus.InvalidInput,
                    Message = "Id in path and body do not match."
                };

            var profileExists = await _context.Profiles.AnyAsync(p => p.Id == employeeUpdateDto.IdProfile.Value);

            if (!profileExists)
            {
                return new ServiceResult<EmployeeGetDto>
                {
                    Status = ServiceResultStatus.InvalidInput,
                    Message = $"Profile with id {employeeUpdateDto.IdProfile.Value} does not exist"
                };
            }
        }

        var dbEmployee = await _context.Employees
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();

        if (dbEmployee is null)
        {
            return new ServiceResult<EmployeeGetDto>
            {
                Status = ServiceResultStatus.NotFound,
                Message = $"Employee with id {id} not found"
            };
        }

        if (employeeUpdateDto.FullName != null)
        {
            dbEmployee.FullName = employeeUpdateDto.FullName;
        }

        if (employeeUpdateDto.Salary.HasValue)
        {
            dbEmployee.Salary = employeeUpdateDto.Salary.Value;
        }

        if (employeeUpdateDto.IdProfile.HasValue)
        {
            dbEmployee.IdProfile = employeeUpdateDto.IdProfile.Value;
        }

        await _context.SaveChangesAsync();

        var updatedEmployeeDto = new EmployeeGetDto
        {
            Id = dbEmployee.Id,
            FullName = dbEmployee.FullName,
            Salary = dbEmployee.Salary,
            IdProfile = dbEmployee.IdProfile,
            NameProfile = await _context.Profiles.Where(p => p.Id == dbEmployee.IdProfile).Select(p => p.Name)
                .FirstOrDefaultAsync()
        };

        return new ServiceResult<EmployeeGetDto>
        {
            Status = ServiceResultStatus.Success,
            Data = updatedEmployeeDto
        };
    }

    public async Task<ServiceResult<bool>> Delete(int id)
    {
        var dbEmployee = await _context.Employees
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();

        if (dbEmployee is null)
            return new ServiceResult<bool>
            {
                Status = ServiceResultStatus.NotFound,
                Message = $"Employee with id {id} not found"
            };

        _context.Employees.Remove(dbEmployee);
        await _context.SaveChangesAsync();

        return new ServiceResult<bool>
        {
            Status = ServiceResultStatus.Success,
            Data = true
        };
    }
}
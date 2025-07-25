using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs.Employee;
using crud_api.Entities;
using crud_api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace crud_api.UnitTests;

public class EmployeeServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly EmployeeService _employeeService;
    private readonly Mock<ILogger<EmployeeService>> _loggerMock;

    public EmployeeServiceTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(dbContextOptions);
        _loggerMock = new Mock<ILogger<EmployeeService>>();
        _employeeService = new EmployeeService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllEmployees()
    {
        // Arrange
        var employee1 = new Employee { Id = 1, FullName = "Keyner de Ávila", IdProfile = 1, Salary = 47500 };
        var employee2 = new Employee { Id = 2, FullName = "Manuel Teherán", IdProfile = 2, Salary = 52000 };

        await _context.AddRangeAsync(employee1, employee2);
        await _context.SaveChangesAsync();

        var employeeQueryParams = new EmployeeQueryParamsDto();


        // Act
        var result = await _employeeService.GetAllPaginatedAndFiltered(employeeQueryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.TotalCount);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoEmployeesExist()
    {
        // Arrange
        var employeeQueryParams = new EmployeeQueryParamsDto();

        // Act
        var result = await _employeeService.GetAllPaginatedAndFiltered(employeeQueryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(0, result.Data.TotalCount);
    }

    [Fact]
    public async Task GetAll_ShouldReturnFilteredEmployees()
    {
        // Arrange
        var employee1 = new Employee { Id = 1, FullName = "Keyner de Ávila", IdProfile = 1, Salary = 47500 };
        var employee2 = new Employee { Id = 2, FullName = "Manual Teherán", IdProfile = 2, Salary = 52000 };

        await _context.AddRangeAsync(employee1, employee2);
        await _context.SaveChangesAsync();

        var employeeQueryParams = new EmployeeQueryParamsDto()
        {
            FullName = "Keyner"
        };

        // Act
        var result = await _employeeService.GetAllPaginatedAndFiltered(employeeQueryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data!.TotalCount);
    }

    [Fact]
    public async Task GetAll_ShouldReturnPaginatedEmployees()
    {
        // Arrange
        // Profiles are needed here because the method projects the profileReference, otherwise it will be null.
        var profile1 = new Profile { Id = 1, Name = "Developer" };
        var profile2 = new Profile { Id = 2, Name = "Analyst" };

        var employee1 = new Employee { Id = 1, FullName = "Keyner de Ávila", IdProfile = 1, Salary = 47500 };
        var employee2 = new Employee { Id = 2, FullName = "Manual Teherán", IdProfile = 2, Salary = 52000 };

        await _context.AddRangeAsync(profile1, profile2, employee1, employee2);
        await _context.SaveChangesAsync();

        var employeeQueryParams = new EmployeeQueryParamsDto
        {
            QueryParams =
            {
                PageNumber = 2,
                PageSize = 1
            }
        };

        // Act
        var result = await _employeeService.GetAllPaginatedAndFiltered(employeeQueryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Single(result.Data.Data!);
        Assert.Equal(employee2.FullName, result.Data.Data![0].FullName);
        Assert.Equal(employee2.Id, result.Data.Data![0].Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnEmployee_WhenEmployeeExists()
    {
        // Arrange
        var profile = new Profile { Id = 1, Name = "Developer" };
        var employee = new Employee { Id = 1, FullName = "Keyner de Ávila", IdProfile = 1, Salary = 47500 };

        await _context.AddRangeAsync(profile, employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _employeeService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(employee.Id, result.Data!.Id);
        Assert.Equal(employee.FullName, result.Data.FullName);
        Assert.Equal(employee.IdProfile, result.Data.IdProfile);
        Assert.Equal(employee.Salary, result.Data.Salary);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        const int idEmployee = 1;

        // Act
        var result = await _employeeService.GetById(idEmployee);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);
        Assert.Equal($"Employee with id {idEmployee} does not exist.", result.Message);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenEmployeeCreated()
    {
        var employeeCreateDto = new EmployeeCreateDto()
        {
            FullName = "Keyner de Ávila",
            IdProfile = 1,
            Salary = 47500
        };

        var profile = new Profile { Id = 1, Name = "Developer" };

        await _context.AddRangeAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _employeeService.Create(employeeCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Created, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(employeeCreateDto.FullName, result.Data!.FullName);
        Assert.Equal(employeeCreateDto.IdProfile, result.Data.IdProfile);
        Assert.Equal(employeeCreateDto.Salary, result.Data.Salary);
    }

    [Fact]
    public async Task Create_ShouldReturnInvalidInput_WhenProfileDoesNotExist()
    {
        // Arrange
        var employeeCreateDto = new EmployeeCreateDto()
        {
            FullName = "Keyner de Ávila",
            IdProfile = 1,
            Salary = 47500
        };

        // Act
        var result = await _employeeService.Create(employeeCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.InvalidInput, result.Status);
        Assert.Null(result.Data);
        Assert.Equal($"Profile with id {employeeCreateDto.IdProfile} does not exist.", result.Message);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        var employeeUpdateDto = new EmployeeUpdateDto()
        {
            Id = 1,
            FullName = "Keyner de Ávila",
            IdProfile = 1,
            Salary = 47500
        };

        // Act
        var result = await _employeeService.Update(1, employeeUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);
        Assert.Equal($"Employee with id 1 not found", result.Message);
    }

    [Fact]
    public async Task Update_ShouldReturnSuccess_WhenEmployeeExists()
    {
        // Arrange
        var profile = new Profile { Id = 1, Name = "Developer" };
        var employee = new Employee { Id = 1, FullName = "Keyner de Ávila", IdProfile = 1, Salary = 47500 };

        await _context.AddRangeAsync(profile, employee);
        await _context.SaveChangesAsync();

        var employeeUpdateDto = new EmployeeUpdateDto()
        {
            Id = 1,
            FullName = "Keyner de Ávila Updated",
            IdProfile = 1,
            Salary = 50000
        };

        // Act
        var result = await _employeeService.Update(1, employeeUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(employeeUpdateDto.FullName, result.Data!.FullName);
        Assert.Equal(employeeUpdateDto.Salary, result.Data.Salary);
    }

    [Fact]
    public async Task Update_ShouldReturnInvalidInput_WhenIdMismatch()
    {
        // Arrange
        var employeeUpdateDto = new EmployeeUpdateDto()
        {
            Id = 2,
            FullName = "Keyner de Ávila",
            IdProfile = 1,
            Salary = 47500
        };

        // Act
        var result = await _employeeService.Update(1, employeeUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.InvalidInput, result.Status);
        Assert.Null(result.Data);
        Assert.Equal("Id in path and body do not match.", result.Message);
    }

    [Fact]
    public async Task Update_ShouldReturnInvalidInput_WhenProfileDoesNotExist()
    {
        // Arrange
        var employee = new Employee { Id = 1, FullName = "Keyner de Ávila", IdProfile = 1, Salary = 47500 };
        await _context.AddAsync(employee);
        await _context.SaveChangesAsync();

        var employeeUpdateDto = new EmployeeUpdateDto()
        {
            Id = 1,
            FullName = "Keyner de Ávila",
            IdProfile = 2,
            Salary = 47500
        };

        // Act
        var result = await _employeeService.Update(1, employeeUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.InvalidInput, result.Status);
        Assert.Null(result.Data);
        Assert.Equal($"Profile with id {employeeUpdateDto.IdProfile} does not exist", result.Message);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        const int idEmployee = 1;

        // Act
        var result = await _employeeService.Delete(idEmployee);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        Assert.False(result.Data);
        Assert.Equal($"Employee with id {idEmployee} not found", result.Message);
    }

    [Fact]
    public async Task Delete_ShouldReturnSuccess_WhenEmployeeExists()
    {
        // Arrange
        var employee = new Employee { Id = 1, FullName = "Keyner de Ávila", IdProfile = 1, Salary = 47500 };
        await _context.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _employeeService.Delete(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Deleted, result.Status);
        Assert.True(result.Data);
    }
}
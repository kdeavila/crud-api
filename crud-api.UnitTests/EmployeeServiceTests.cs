using crud_api.Context;
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
}
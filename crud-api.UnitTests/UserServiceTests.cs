using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs.Common;
using crud_api.DTOs.User;
using crud_api.Entities;
using crud_api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace crud_api.UnitTests;

public class UserServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(dbContextOptions);
        var loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_context, loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllUsers()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password_example");

        var user1 = new User
        {
            Email = "admin@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Admin
        };

        var user2 = new User
        {
            Email = "viewe@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Viewer
        };

        await _context.AddRangeAsync(user1, user2);
        await _context.SaveChangesAsync();

        var userQueryParams = new UserQueryParamsDto();

        // Act
        var result = await _userService.GetAllPaginatedAndFiltered(userQueryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(user1.Id, result.Data.Data![0].Id);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoUserExists()
    {
        // Arrange
        var userQueryParams = new UserQueryParamsDto();

        // Act
        var result = await _userService.GetAllPaginatedAndFiltered(userQueryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(0, result.Data!.TotalCount);
    }

    [Fact]
    public async Task GetAll_ShouldReturnPaginatedAndFilteredUsers()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password_example");

        var user1 = new User
        {
            Email = "admin@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Admin
        };

        var user2 = new User
        {
            Email = "viewer@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Viewer
        };

        await _context.AddRangeAsync(user1, user2);
        await _context.SaveChangesAsync();

        var userQueryParams = new UserQueryParamsDto
        {
            Email = "viewer"
        };

        // Act
        var result = await _userService.GetAllPaginatedAndFiltered(userQueryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data!.TotalCount);
        Assert.Equal(user2.Id, result.Data.Data![0].Id);
    }

    [Fact]
    public async Task GetAll_FilteredPaginated_ShouldHandleEmptyPage()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password_example");

        var user1 = new User
        {
            Email = "admin@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Admin
        };

        var user2 = new User
        {
            Email = "viewer@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Viewer
        };

        await _context.AddRangeAsync(user1, user2);
        await _context.SaveChangesAsync();

        var queryParams = new UserQueryParamsDto
        {
            Email = "viewer",
            QueryParams = new QueryParamsDto
            {
                PageNumber = 2,
                PageSize = 1
            }
        };

        // Act
        var result = await _userService.GetAllPaginatedAndFiltered(queryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.TotalCount);
        Assert.Empty(result.Data.Data!);
    }

    [Fact]  
    public async Task GetById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password_example");

        const int id = 1;
        var user = new User
        {
            Id = id,
            Email = "email@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Admin
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.GetById(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(id, result.Data!.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        const int id = 1;

        // Act
        var result = await _userService.GetById(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);
        Assert.Equal($"User with id {id} does not exist.", result.Message);
    }

    [Fact]
    public async Task Update_ShouldReturnUpdated_WhenUserUpdated()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password_example");

        const int id = 1;
        var user = new User
        {
            Id = id,
            Email = "admin@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Admin
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        var userUpdated = new UserUpdateDto()
        {
            Id = id,
            Email = "admin@example.com",
            Role = UserRole.Viewer
        };

        // Act
        var result = await _userService.Update(id, userUpdated);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(id, result.Data!.Id);
        Assert.Equal(UserRole.Viewer, result.Data.Role);
    }
}
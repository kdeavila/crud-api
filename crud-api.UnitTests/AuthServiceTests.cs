using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs.Auth;
using crud_api.Entities;
using crud_api.Services;
using crud_api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace crud_api.UnitTests;

public class AuthServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    private readonly Mock<IOptions<JwtSettings>> _options;

    public AuthServiceTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(dbContextOptions);
        _options = new Mock<IOptions<JwtSettings>>();

        var jwtSettings = new JwtSettings
        {
            SecretKey = "a_super_secret_key_that_is_long_enough_for_hs256",
            Issuer = "test_issuer",
            Audience = "test_audience",
            ExpirationMinutes = 60
        };

        _options.Setup(o => o.Value).Returns(jwtSettings);
        _authService = new AuthService(_context, _options.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task Login_ShouldReturnJwt_WithValidCredentials()
    {
        // Arrange
        const string password = "password_example";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User()
        {
            Email = "email@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Admin
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        var userLoginDto = new UserLoginDto
        {
            Email = "email@example.com",
            Password = password
        };

        // Act
        var result = await _authService.Login(userLoginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.False(string.IsNullOrEmpty(result.JWT));
        Assert.Equal("Login successful", result.Message);
    }

    [Fact]
    public async Task Login_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var userLoginDto = new UserLoginDto
        {
            Email = "email@example.com",
            Password = "password_example"
        };

        // Act
        var result = await _authService.Login(userLoginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        Assert.Equal("User not found", result.Message);
        Assert.Null(result.JWT);
    }

    [Fact]
    public async Task Login_ShouldReturnInvalidInput_WhenThePasswordIsInvalid()
    {
        // Arrange
        const string password = "password_example";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User()
        {
            Email = "email@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Admin
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        var userLoginDto = new UserLoginDto
        {
            Email = "email@example.com",
            Password = "password_invalid"
        };

        // Act
        var result = await _authService.Login(userLoginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.InvalidInput, result.Status);
        Assert.Equal("Invalid password", result.Message);
        Assert.Null(result.JWT);
    }

    [Fact]
    public async Task Login_ShouldReturnJwt_InsensitiveEmailCase()
    {
        // Arrange
        const string password = "password_example";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User()
        {
            Email = "EMAIL@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Admin
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        var userLoginDto = new UserLoginDto
        {
            Email = "email@example.com",
            Password = password
        };

        // Act
        var result = await _authService.Login(userLoginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.False(string.IsNullOrEmpty(result.JWT));
        Assert.Equal("Login successful", result.Message);
    }

    [Fact]
    public async Task Register_ShouldReturnSuccess_WhenUserIsCreated()
    {
        // Arrange  
        var userRegisterDto = new UserRegisterDto
        {
            Email = "admin@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("password_example"),
            Role = UserRole.Admin
        };

        // Act
        var result = await _authService.Register(userRegisterDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.Equal("Registration successful", result.Message);
        Assert.True(string.IsNullOrEmpty(result.JWT));

        var userInDb = await _context.Users.SingleOrDefaultAsync(u => u.Email == userRegisterDto.Email);
        Assert.NotNull(userInDb);
        Assert.True(BCrypt.Net.BCrypt.Verify(userRegisterDto.Password, userInDb.PasswordHash));
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenUserAlreadyExists()
    {
        // Arrange
        const string password = "password_example";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Email = "admin@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.Admin
        };
        
        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        var userRegisterDto = new UserRegisterDto
        {
            Email = "admin@example.com",
            Password = password
        };
        
        // Act
        var result = await _authService.Register(userRegisterDto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Conflict, result.Status);
        Assert.Equal("User already exists", result.Message);
        Assert.Null(result.JWT);
    }
    
    
}
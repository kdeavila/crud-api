using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs.Profile;
using crud_api.Entities;
using crud_api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace crud_api.UnitTests;

public class ProfileServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProfileService _profileService;
    private readonly Mock<ILogger<ProfileService>> _loggerMock;

    public ProfileServiceTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(dbContextOptions);
        _loggerMock = new Mock<ILogger<ProfileService>>();
        _profileService = new ProfileService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllProfiles()
    {
        // Arrange
        var profile1 = new Profile { Id = 1, Name = "Developer" };
        var profile2 = new Profile { Id = 2, Name = "Analyst" };
        await _context.AddRangeAsync(profile1, profile2);
        await _context.SaveChangesAsync();

        var profileQueryParams = new ProfileQueryParams();

        // Act
        var result = await _profileService.GetAllPaginatedAndFiltered(profileQueryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoProfilesExist()
    {
        // Arrange
        var profileQueryParams = new ProfileQueryParams();

        // Act
        var result = await _profileService.GetAllPaginatedAndFiltered(profileQueryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(0, result.Data!.TotalCount);
    }

    [Fact]
    public async Task GetAll_ShouldReturnFilteredProfiles()
    {
        // Arrange
        var profile1 = new Profile { Id = 1, Name = "Developer" };
        var profile2 = new Profile { Id = 2, Name = "Analyst" };

        await _context.AddRangeAsync(profile1, profile2);
        await _context.SaveChangesAsync();

        var profileQueryParams = new ProfileQueryParams()
        {
            Name = "Analyst"
        };

        // Act
        var result = await _profileService.GetAllPaginatedAndFiltered(profileQueryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data!.TotalCount);
        Assert.Equal(2, result.Data.Data![0].Id);
        Assert.Equal("Analyst", result.Data!.Data[0].Name);
    }

    [Fact]
    public async Task GetAll_ShouldReturnPaginatedProfiles()
    {
        // Arrange
        var profile1 = new Profile { Id = 1, Name = "Developer" };
        var profile2 = new Profile { Id = 2, Name = "Analyst" };

        await _context.AddRangeAsync(profile1, profile2);
        await _context.SaveChangesAsync();

        var profileQueryParams = new ProfileQueryParams()
        {
            QueryParams =
            {
                PageNumber = 2,
                PageSize = 1
            }
        };

        // Act
        var result = await _profileService.GetAllPaginatedAndFiltered(profileQueryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Single(result.Data.Data!);
        Assert.Equal(2, result.Data.Data![0].Id);
        Assert.Equal("Analyst", result.Data!.Data[0].Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnProfile_WhenProfileExists()
    {
        // Arrange
        var testProfile = new Profile { Id = 1, Name = "Developer" };
        await _context.AddAsync(testProfile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _profileService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data!.Id);
        Assert.Equal("Developer", result.Data.Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        const int idProfile = 1;

        // Act
        var result = await _profileService.GetById(idProfile);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);
        Assert.Equal($"Profile with id {idProfile} does not exist.", result.Message);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenProfileCreated()
    {
        // Arrange
        var profileCreateDto = new ProfileCreateDto() { Name = "Developer" };

        // Act
        var result = await _profileService.Create(profileCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Created, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data!.Id);
        Assert.Equal("Developer", result.Data.Name);
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenProfileAlreadyExists()
    {
        // Arrange
        var profile1 = new Profile { Id = 1, Name = "Developer" };
        await _context.AddAsync(profile1);
        await _context.SaveChangesAsync();
        
        var profileCreateDto = new ProfileCreateDto() { Name = "Developer" };
        
        // Act
        var result = await _profileService.Create(profileCreateDto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Conflict, result.Status);
        Assert.Null(result.Data);
        Assert.Equal($"Profile with name {profileCreateDto.Name} already exists", result.Message);
    }

    [Fact]
    public async Task Update_ShouldReturnUpdated_WhenProfileUpdated()
    {
        // Arrange
        var profile1 = new Profile { Id = 1, Name = "Developer" };
        await _context.AddAsync(profile1);
        await _context.SaveChangesAsync();
        
        var profileUpdateDto = new ProfileUpdateDto() { Id = 1, Name = "Analyst" };
        
        // Act
        var result = await _profileService.Update(1, profileUpdateDto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data!.Id);
        Assert.Equal("Analyst", result.Data.Name);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        var profileUpdateDto = new ProfileUpdateDto() { Id = 1, Name = "Analyst" };
        
        // Act
        var result = await _profileService.Update(1, profileUpdateDto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        Assert.Equal($"Profile with id {profileUpdateDto.Id} does not exist.", result.Message);
        Assert.Null(result.Data);
    }
    
    [Fact]
    public async Task Update_ShouldReturnConflict_WhenProfileAlreadyExists()
    {
        // Arrange
        var profile1 = new Profile { Id = 1, Name = "Developer" };
        await _context.AddAsync(profile1);
        
        var profile2 = new Profile { Id = 2, Name = "Analyst" };
        await _context.AddAsync(profile2);
        await _context.SaveChangesAsync();
        
        var profileUpdateDto = new ProfileUpdateDto() { Id = 1, Name = "Analyst" };
        
        // Act
        var result = await _profileService.Update(1, profileUpdateDto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Conflict, result.Status);
        Assert.Equal($"Profile with name {profileUpdateDto.Name} already exists", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Delete_ShouldReturnDeleted_WhenProfileDeleted()
    {
        // Arrange
        var profile1 = new Profile { Id = 1, Name = "Developer" };
        await _context.AddAsync(profile1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _profileService.Delete(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.Deleted, result.Status);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenProfileDoesNotExist()
    {
        // Arrange

        // Act
        var result = await _profileService.Delete(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        Assert.Equal("Profile with id 1 does not exist.", result.Message);
    }
}
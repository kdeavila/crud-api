using crud_api.Common;
using crud_api.Context;
using crud_api.Entities;
using crud_api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace crud_api.UnitTests;

public class ProfileServiceTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    [Fact]
    public async Task GetById_ShouldReturnProfile_WhenProfileExists()
    {
        // Arrange
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "GetById_ShouldReturnProfile_WhenProfileExists")
            .Options;
        
       await using var context = new AppDbContext(dbContextOptions);

        var testProfile = new Profile { Id = 1, Name = "Developer" };
        await context.AddAsync(testProfile);
        await context.SaveChangesAsync();

        var loggerMock = new Mock<ILogger<ProfileService>>();

        var profileService = new ProfileService(context, loggerMock.Object);
        
        // Act
        var result = await profileService.GetById(1);
        
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
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "GetById_ShouldReturnNotFound_WhenProfileDoesNotExist")
            .Options;

        await using var context = new AppDbContext(dbContextOptions);
        
        var loggerMock = new Mock<ILogger<ProfileService>>();

        var profileService = new ProfileService(context, loggerMock.Object);
        
        // Act
        const int idProfile = 1;
        var result = await profileService.GetById(idProfile);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);
        Assert.Equal($"Profile with id {idProfile} does not exist.", result.Message);
    }
}

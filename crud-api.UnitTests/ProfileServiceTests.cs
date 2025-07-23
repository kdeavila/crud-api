﻿using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs.Profile;
using crud_api.Entities;
using crud_api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace crud_api.UnitTests;

public class ProfileServiceTests
{
    [Fact]
    public async Task GetAll_ShouldReturnAllProfiles()
    {
        // Arrange
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAll_ShouldReturnAllProfiles")
            .Options;

        await using var context = new AppDbContext(dbContextOptions);
        
        var profile1 = new Profile { Id = 1, Name = "Developer" };
        var profile2 = new Profile { Id = 2, Name = "Analyst" };
        await context.AddRangeAsync(profile1, profile2);
        await context.SaveChangesAsync();

        var profileQueryParams = new ProfileQueryParams();

        var loggerMock = new Mock<ILogger<ProfileService>>();

        var profileService = new ProfileService(context, loggerMock.Object);
        
        // Act
        var result = await profileService.GetAllPaginatedAndFiltered(profileQueryParams);
        
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
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAll_ShouldReturnEmptyList_WhenNoProfilesExist")
            .Options;
        
        await using var context = new AppDbContext(dbContextOptions);
        
        var profileQueryParams = new ProfileQueryParams();

        var loggerMock = new Mock<ILogger<ProfileService>>();
        
        var profileService = new ProfileService(context, loggerMock.Object);
        
        // Act
        var result = await profileService.GetAllPaginatedAndFiltered(profileQueryParams);
        
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
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAll_ShouldReturnFilteredProfiles")
            .Options;
        
        await using var context = new AppDbContext(dbContextOptions);
        
        var profile1 = new Profile { Id = 1, Name = "Developer" };
        var profile2 = new Profile { Id = 2, Name = "Analyst" };
        
        await context.AddRangeAsync(profile1, profile2);
        await context.SaveChangesAsync();

        var profileQueryParams = new ProfileQueryParams()
        {
            Name = "Analyst"
        };
        
        var loggerMock = new Mock<ILogger<ProfileService>>();
        
        var profileService = new ProfileService(context, loggerMock.Object);
        
        // Act
        var result = await profileService.GetAllPaginatedAndFiltered(profileQueryParams);
        
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
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAll_ShouldReturnPaginatedProfiles")
            .Options;
        
        await using var context = new AppDbContext(dbContextOptions);
        
        var profile1 = new Profile { Id = 1, Name = "Developer" };
        var profile2 = new Profile { Id = 2, Name = "Analyst" };
        
        await context.AddRangeAsync(profile1, profile2);
        await context.SaveChangesAsync();

        var profileQueryParams = new ProfileQueryParams()
        {
            QueryParams =
            {
                PageNumber = 2,
                PageSize = 1
            }
        };
        
        var loggerMock = new Mock<ILogger<ProfileService>>();
        
        var profileService = new ProfileService(context, loggerMock.Object);
        
        // Act
        var result = await profileService.GetAllPaginatedAndFiltered(profileQueryParams);
        
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

using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs.Common;
using crud_api.DTOs.Profile;
using crud_api.Entities;
using crud_api.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class ProfileService(AppDbContext context, ILogger<ProfileService> logger) : IProfileService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<ProfileService> _logger = logger;

    public async Task<ServiceResult<PaginatedResultDto<ProfileGetDto>>> GetAllPaginatedAndFiltered(
        ProfileQueryParams profileQueryParams)
    {
        var query = _context.Profiles.AsQueryable();

        if (!string.IsNullOrEmpty(profileQueryParams.Name))
        {
            query = query.Where(p => p.Name.Contains(profileQueryParams.Name));
        }

        var sortByLower = profileQueryParams.QueryParams.SortBy?.ToLower();
        var orderLower = profileQueryParams.QueryParams.Order?.ToLower();

        switch (sortByLower)
        {
            case "name":
                query = (orderLower == "desc")
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name);
                break;
            default:
                query = (orderLower == "desc")
                    ? query.OrderByDescending(p => p.Id)
                    : query.OrderBy(p => p.Id);
                break;
        }

        var totalCount = await query.CountAsync();

        query = query
            .Skip((profileQueryParams.QueryParams.PageNumber - 1) * profileQueryParams.QueryParams.PageSize)
            .Take(profileQueryParams.QueryParams.PageSize);

        var paginatedProfiles = await query
            .Select(p => new ProfileGetDto
            {
                Id = p.Id,
                Name = p.Name
            })
            .ToListAsync();

        var paginatedResult = new PaginatedResultDto<ProfileGetDto>
        {
            Data = paginatedProfiles,
            TotalCount = totalCount,
            PageNumber = profileQueryParams.QueryParams.PageNumber,
            PageSize = profileQueryParams.QueryParams.PageSize
        };

        var result = new ServiceResult<PaginatedResultDto<ProfileGetDto>>
        {
            Data = paginatedResult,
            Status = ServiceResultStatus.Success
        };

        return result;
    }

    public async Task<ServiceResult<ProfileGetDto>> GetById(int id)
    {
        var profileDto = await _context.Profiles
            .Where(p => p.Id == id)
            .Select(p => new ProfileGetDto
            {
                Id = p.Id,
                Name = p.Name
            })
            .FirstOrDefaultAsync();

        if (profileDto is null)
            return new ServiceResult<ProfileGetDto>
            {
                Status = ServiceResultStatus.NotFound,
                Message = $"Profile with id {id} does not exist."
            };

        return new ServiceResult<ProfileGetDto>
        {
            Data = profileDto,
            Status = ServiceResultStatus.Success
        };
    }

    public async Task<ServiceResult<ProfileGetDto>> Create(ProfileCreateDto profileCreateDto)
    {
        try
        {
            var nameExists = await _context.Profiles.AnyAsync(p => p.Name == profileCreateDto.Name);
            if (nameExists)
            {
                return new ServiceResult<ProfileGetDto>
                {
                    Status = ServiceResultStatus.Conflict,
                    Message = $"Profile with name {profileCreateDto.Name} already exists"
                };
            }

            var dbProfile = new Profile
            {
                Name = profileCreateDto.Name
            };

            await _context.Profiles.AddAsync(dbProfile);
            await _context.SaveChangesAsync();

            var createdProfileDto = new ProfileGetDto
            {
                Id = dbProfile.Id,
                Name = dbProfile.Name
            };

            return new ServiceResult<ProfileGetDto>
            {
                Status = ServiceResultStatus.Created,
                Data = createdProfileDto
            };
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException is SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case 2627 or 2601:
                        _logger.LogWarning(dbEx,
                            "DbUpdateException: Unique constraint violation during profile creation. Details: {SqlErrorMessage}",
                            sqlEx.Message);
                        return new ServiceResult<ProfileGetDto>
                        {
                            Status = ServiceResultStatus.Conflict,
                            Message = "A profile with the same unique identifier already exists."
                        };
                    default:
                        _logger.LogError(dbEx,
                            "DbUpdateException: An unhandled SQL error occurred during profile creation. SQL Error Number: {SqlErrorNumber}, Message: {SqlErrorMessage}",
                            sqlEx.Number, sqlEx.Message);
                        return new ServiceResult<ProfileGetDto>
                        {
                            Status = ServiceResultStatus.Error,
                            Message = "An unexpected database error occurred. Please try again later."
                        };
                }
            }
            else
            {
                _logger.LogError(dbEx,
                    "DbUpdateException: An unexpected database update error occurred during profile creation. Message: {DbUpdateMessage}",
                    dbEx.Message);
                return new ServiceResult<ProfileGetDto>
                {
                    Status = ServiceResultStatus.Error,
                    Message = "An unexpected database error occurred. Please try again later."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during profile creation.");
            return new ServiceResult<ProfileGetDto>
            {
                Status = ServiceResultStatus.Error,
                Message = "An unexpected error occurred. Please try again later."
            };
        }
    }

    public async Task<ServiceResult<ProfileGetDto>> Update(int id, ProfileUpdateDto profileUpdateDto)
    {
        if (id != profileUpdateDto.Id)
            return new ServiceResult<ProfileGetDto>
            {
                Status = ServiceResultStatus.InvalidInput,
                Message = "Id in path and body do not match."
            };

        try
        {
            var dbProfile = await _context.Profiles
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (dbProfile is null)
            {
                return new ServiceResult<ProfileGetDto>
                {
                    Status = ServiceResultStatus.NotFound,
                    Message = $"Profile with id {id} does not exist."
                };
            }

            var nameExists = await _context.Profiles.AnyAsync(p => p.Name == profileUpdateDto.Name && p.Id != id);
            if (nameExists)
            {
                return new ServiceResult<ProfileGetDto>
                {
                    Status = ServiceResultStatus.Conflict,
                    Message = $"Profile with name {profileUpdateDto.Name} already exists"
                };
            }

            dbProfile.Name = profileUpdateDto.Name;
            await _context.SaveChangesAsync();

            var updatedProfileDto = new ProfileGetDto
            {
                Id = dbProfile.Id,
                Name = dbProfile.Name
            };

            return new ServiceResult<ProfileGetDto>
            {
                Status = ServiceResultStatus.Success,
                Data = updatedProfileDto
            };
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException is SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case 2627 or 2601:
                        _logger.LogWarning(dbEx,
                            "DbUpdateException (Update): Unique constraint violation during profile update. SQL Error: {SqlErrorMessage}",
                            sqlEx.Message);
                        return new ServiceResult<ProfileGetDto>
                        {
                            Status = ServiceResultStatus.Conflict,
                            Message = "A profile with the same unique identifier already exists."
                        };
                    default:
                        _logger.LogError(dbEx,
                            "DbUpdateException (Update): An unhandled SQL error occurred during profile update. SQL Error Number: {SqlErrorNumber}, Message: {SqlErrorMessage}",
                            sqlEx.Number, sqlEx.Message);
                        return new ServiceResult<ProfileGetDto>
                        {
                            Status = ServiceResultStatus.Error,
                            Message = "An unexpected database error occurred. Please try again later."
                        };
                }
            }
            else
            {
                _logger.LogError(dbEx,
                    "DbUpdateException: An unexpected database update error occurred during profile update. Message: {DbUpdateMessage}",
                    dbEx.Message);
                return new ServiceResult<ProfileGetDto>
                {
                    Status = ServiceResultStatus.Error,
                    Message = "An unexpected database error occurred. Please try again later."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during profile update.");
            return new ServiceResult<ProfileGetDto>
            {
                Status = ServiceResultStatus.Error,
                Message = "An unexpected error occurred. Please try again later."
            };
        }
    }

    public async Task<ServiceResult<bool>> Delete(int id)
    {
        try
        {
            var dbProfile = await _context.Profiles
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (dbProfile is null)
                return new ServiceResult<bool>
                {
                    Status = ServiceResultStatus.NotFound,
                    Message = $"Profile with id {id} does not exist."
                };

            _context.Profiles.Remove(dbProfile);
            await _context.SaveChangesAsync();

            return new ServiceResult<bool>
            {
                Status = ServiceResultStatus.Deleted,
                Data = true
            };
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException is SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case 547:
                        _logger.LogWarning(dbEx,
                            "DbUpdateException (Delete): Foreign key violation during profile deletion. Details: {SqlErrorMessage}",
                            sqlEx.Message);
                        return new ServiceResult<bool>
                        {
                            Status = ServiceResultStatus.Conflict,
                            Message = "The profile cannot be deleted because it is referenced by other entities." 
                        };
                    default:
                        _logger.LogError(dbEx,
                            "DbUpdateException (Delete): An unhandled SQL error occurred during profile deletion. SQL Error Number: {SqlErrorNumber}, Message: {SqlErrorMessage}",
                            sqlEx.Number, sqlEx.Message);
                        return new ServiceResult<bool>
                        {
                            Status = ServiceResultStatus.Error,
                            Message = "An unexpected database error occurred. Please try again later."
                        };
                }
            }
            else
            {
                _logger.LogError(dbEx,
                    "DbUpdateException (Delete): An unexpected database update error occurred during profile deletion. Message: {DbUpdateMessage}",
                    dbEx.Message);
                return new ServiceResult<bool>
                {
                    Status = ServiceResultStatus.Error,
                    Message = "An unexpected database error occurred during update. Please try again later."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during profile deletion.");
            return new ServiceResult<bool>
            {
                Status = ServiceResultStatus.Error,
                Message = "An unexpected error occurred. Please try again later."
            };
        }
    }
}
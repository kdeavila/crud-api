using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs.Common;
using crud_api.DTOs.Profile;
using crud_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class ProfileService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

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
            case "id":
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

        if (profileDto is null) return new ServiceResult<ProfileGetDto>
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
            Status = ServiceResultStatus.Success,
            Data = createdProfileDto
        };
    }

    public async Task<ServiceResult<ProfileGetDto>> Update(int id, ProfileUpdateDto profileUpdateDto)
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

    public async Task<ServiceResult<bool>> Delete(int id)
    {
        var dbProfile = await _context.Profiles
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();

        if (dbProfile is null) return new ServiceResult<bool>
        {
            Status = ServiceResultStatus.NotFound,
            Message = $"Profile with id {id} does not exist."
        };

        _context.Profiles.Remove(dbProfile);
        await _context.SaveChangesAsync();
        
        return new ServiceResult<bool>
        {
            Status = ServiceResultStatus.Success,
            Data = true
        };
    }
}
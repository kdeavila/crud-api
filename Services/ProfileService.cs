using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs;
using crud_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class ProfileService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<ProfileDto>> List()
    {
        var profiles = await _context.Profiles.ToListAsync();

        return profiles.Select(p => new ProfileDto { Id = p.Id, Name = p.Name, }).ToList();
    }

    public async Task<ProfileDto?> GetById(int id)
    {
        var dtoProfile = await _context.Profiles
            .Where(p => p.Id == id)
            .Select(p => new ProfileDto
            {
                Id = p.Id,
                Name = p.Name
            })
            .FirstOrDefaultAsync();
        
        return dtoProfile;
    }

    public async Task<ServiceResult<ProfileDto>> Create(ProfileDto dtoProfile)
    {
        var nameExists = await _context.Profiles.AnyAsync(p => p.Name == dtoProfile.Name);

        if (nameExists)
        {
            return new ServiceResult<ProfileDto>
            {
                Status = ServiceResultStatus.Conflict,
                Message = $"Profile with name {dtoProfile.Name} already exists"
            };
        }

        var dbProfile = new Profile
        {
            Name = dtoProfile.Name
        };
        
        await _context.Profiles.AddAsync(dbProfile);
        await _context.SaveChangesAsync();

        var createdProfileDto = new ProfileDto
        {
            Id = dbProfile.Id,
            Name = dbProfile.Name
        };

        return new ServiceResult<ProfileDto>
        {
            Status = ServiceResultStatus.Success,
            Data = createdProfileDto
        };
    }

    public async Task<ServiceResult<ProfileDto>> Update(int id, ProfileDto dtoProfile)
    {
        var dbProfile = await _context.Profiles
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        
        if (dbProfile is null)
        {
            return new ServiceResult<ProfileDto>
            {
                Status = ServiceResultStatus.NotFound,
                Message = $"Profile with id {id} does not exist."
            };
        }
        
        var nameExists = await _context.Profiles.AnyAsync(p => p.Name == dtoProfile.Name && p.Id != id);
        if (nameExists)
        {
            return new ServiceResult<ProfileDto>
            {
                Status = ServiceResultStatus.Conflict,
                Message = $"Profile with name {dtoProfile.Name} already exists"
            };
        }
        
        dbProfile.Name = dtoProfile.Name;
        await _context.SaveChangesAsync();

        var updatedProfileDto = new ProfileDto
        {
            Id = dbProfile.Id,
            Name = dbProfile.Name
        };

        return new ServiceResult<ProfileDto>
        {
            Status = ServiceResultStatus.Success,
            Data = updatedProfileDto
        };
    }

    public async Task<bool> Delete(int id)
    {
        var dbProfile = await _context.Profiles
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        
        if (dbProfile is null) return false;
        
        _context.Profiles.Remove(dbProfile);
        await _context.SaveChangesAsync();
        return true;
    }
}
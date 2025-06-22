using crud_api.Context;
using crud_api.DTOS;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class ProfileService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<ProfileDTO>> List()
    {
        var dtoList = new List<ProfileDTO>();
        var profiles = await _context.Profiles.ToListAsync();

        foreach (var p in profiles)
        {
            dtoList.Add(new ProfileDTO
            {
                Id = p.Id,
                Name = p.Name,
            });
        }

        return dtoList;
    }
}
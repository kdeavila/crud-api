using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs.Common;
using crud_api.DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class UserService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<ServiceResult<PaginatedResultDto<UserDto>>> GetAllPaginatedAndFiltered(
        UserQueryParamsDto userQueryParamsDto)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(userQueryParamsDto.Email))
        {
            query = query.Where(u => u.Email.Contains(userQueryParamsDto.Email));
        }

        if (userQueryParamsDto.Role.HasValue)
        {
            query = query.Where(u => u.Role == userQueryParamsDto.Role);
        }

        var sortByLower = userQueryParamsDto.QueryParams.SortBy?.ToLower();
        var orderLower = userQueryParamsDto.QueryParams.Order?.ToLower();

        switch (sortByLower)
        {
            case "email":
                query = (orderLower == "desc")
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email);
                break;
            case "id":
            default:
                query = (orderLower == "desc")
                    ? query.OrderByDescending(u => u.Id)
                    : query.OrderBy(u => u.Id);
                break;
        }

        var totalCount = await query.CountAsync();

        query = query
            .Skip((userQueryParamsDto.QueryParams.PageNumber - 1) * userQueryParamsDto.QueryParams.PageSize)
            .Take(userQueryParamsDto.QueryParams.PageSize);

        var paginatedUser = await query
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();

        var paginatedResult = new PaginatedResultDto<UserDto>
        {
            Data = paginatedUser,
            TotalCount = totalCount,
            PageNumber = userQueryParamsDto.QueryParams.PageNumber,
            PageSize = userQueryParamsDto.QueryParams.PageSize
        };

        var result = new ServiceResult<PaginatedResultDto<UserDto>>
        {
            Data = paginatedResult,
            Status = ServiceResultStatus.Success
        };

        return result;
    }

    public async Task<ServiceResult<UserDto>> GetById(int id)
    {
        var userDto = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role
            })
            .FirstOrDefaultAsync();

        if (userDto is null)
        {
            return new ServiceResult<UserDto>
            {
                Status = ServiceResultStatus.NotFound,
                Message = $"User with id {id} not found"
            };
        }

        var result = new ServiceResult<UserDto>
        {
            Data = userDto,
            Status = ServiceResultStatus.Success
        };

        return result;
    }
}
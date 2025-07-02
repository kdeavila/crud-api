using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs.Common;
using crud_api.DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class UserService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<ServiceResult<PaginatedResultDto<UserGetDto>>> GetAllPaginatedAndFiltered(
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
            case "role":
                query = (orderLower == "desc")
                    ? query.OrderByDescending(u => u.Role)
                    : query.OrderBy(u => u.Role);
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
            .Select(u => new UserGetDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();

        var paginatedResult = new PaginatedResultDto<UserGetDto>
        {
            Data = paginatedUser,
            TotalCount = totalCount,
            PageNumber = userQueryParamsDto.QueryParams.PageNumber,
            PageSize = userQueryParamsDto.QueryParams.PageSize
        };

        var result = new ServiceResult<PaginatedResultDto<UserGetDto>>
        {
            Data = paginatedResult,
            Status = ServiceResultStatus.Success
        };

        return result;
    }

    public async Task<ServiceResult<UserGetDto>> GetById(int id)
    {
        var userDto = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserGetDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role
            })
            .FirstOrDefaultAsync();

        if (userDto is null)
        {
            return new ServiceResult<UserGetDto>
            {
                Status = ServiceResultStatus.NotFound,
                Message = $"User with id {id} not found"
            };
        }

        var result = new ServiceResult<UserGetDto>
        {
            Data = userDto,
            Status = ServiceResultStatus.Success
        };

        return result;
    }

    public async Task<ServiceResult<UserGetDto>> Update(int id, UserUpdateDto userUpdateDto)
    {
        var dbUser = await _context.Users
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync();

        if (dbUser is null)
        {
            return new ServiceResult<UserGetDto>
            {
                Status = ServiceResultStatus.NotFound,
                Message = $"User with id {id} not found"
            };
        }

        if (userUpdateDto.Email != null)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == userUpdateDto.Email && u.Id != id);

            if (emailExists)
                return new ServiceResult<UserGetDto>
                {
                    Status = ServiceResultStatus.Conflict,
                    Message = $"Email {userUpdateDto.Email} already exists"
                };

            dbUser.Email = userUpdateDto.Email;
        }

        if (userUpdateDto.Role.HasValue)
        {
            dbUser.Role = userUpdateDto.Role.Value;
        }

        await _context.SaveChangesAsync();
        var updatedUserDto = new UserGetDto
        {
            Id = dbUser.Id,
            Email = dbUser.Email,
            Role = dbUser.Role
        };

        var result = new ServiceResult<UserGetDto>
        {
            Data = updatedUserDto,
            Status = ServiceResultStatus.Success
        };

        return result;
    }

    public async Task<ServiceResult<bool>> Delete(int id)
    {
        var dbUser = await _context.Users
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync();

        if (dbUser is null)
        {
            return new ServiceResult<bool>
            {
                Status = ServiceResultStatus.NotFound,
                Message = $"User with id {id} not found"
            };
        }

        _context.Users.Remove(dbUser);
        await _context.SaveChangesAsync();

        var result = new ServiceResult<bool>
        {
            Status = ServiceResultStatus.Success,
            Data = true
        };

        return result;
    }
}
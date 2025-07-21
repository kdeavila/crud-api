using crud_api.Common;
using crud_api.Context;
using crud_api.DTOs.Common;
using crud_api.DTOs.User;
using crud_api.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class UserService(AppDbContext context, ILogger<UserService> logger) : IUserService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<UserService> _logger = logger;

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
        if (id != userUpdateDto.Id)
            return new ServiceResult<UserGetDto>
            {
                Status = ServiceResultStatus.InvalidInput,
                Message = "Id in path and body do not match."
            };

        try
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

            return new ServiceResult<UserGetDto>
            {
                Data = updatedUserDto,
                Status = ServiceResultStatus.Success
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
                            "DbUpdateException (Update): Unique constraint violation during user update. SQL Error: {SqlErrorMessage}",
                            sqlEx.Message);
                        return new ServiceResult<UserGetDto>
                        {
                            Status = ServiceResultStatus.Conflict,
                            Message = "A user with the same unique identifier already exists."
                        };
                    default:
                        _logger.LogError(dbEx,
                            "DbUpdateException (Update): An unhandled SQL error occurred during user update. SQL Error Number: {SqlErrorNumber}, Message: {SqlErrorMessage}",
                            sqlEx.Number, sqlEx.Message);
                        return new ServiceResult<UserGetDto>
                        {
                            Status = ServiceResultStatus.Error,
                            Message = "An unexpected database error occurred. Please try again later."
                        };
                }
            }
            else
            {
                _logger.LogError(dbEx,
                    "DbUpdateException: An unexpected database update error occurred during user update. Message: {DbUpdateMessage}",
                    dbEx.Message);
                return new ServiceResult<UserGetDto>
                {
                    Status = ServiceResultStatus.Error,
                    Message = "An unexpected database error occurred. Please try again later."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during user update.");
            return new ServiceResult<UserGetDto>
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
            var dbUser = await _context.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (dbUser is null)
                return new ServiceResult<bool>
                {
                    Status = ServiceResultStatus.NotFound,
                    Message = $"User with id {id} not found."
                };

            _context.Users.Remove(dbUser);
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
                            "DbUpdateException (Delete): Foreign key violation during user deletion. SQL Error: {SqlErrorMessage}",
                            sqlEx.Message);
                        return new ServiceResult<bool>
                        {
                            Status = ServiceResultStatus.Conflict,
                            Message =
                                "The user cannot be deleted because it is referenced by other entities (e.g., employees, logs)."
                        };
                    default:
                        _logger.LogError(dbEx,
                            "DbUpdateException (Delete): An unhandled SQL error occurred during user deletion. SQL Error Number: {SqlErrorNumber}, Message: {SqlErrorMessage}",
                            sqlEx.Number, sqlEx.Message);
                        return new ServiceResult<bool>
                        {
                            Status = ServiceResultStatus.Error,
                            Message = "An unexpected database error occurred during deletion. Please try again later."
                        };
                }
            }
            else
            {
                _logger.LogError(dbEx,
                    "DbUpdateException (Delete): An unexpected database update error occurred during user deletion. Message: {DbUpdateMessage}",
                    dbEx.Message);
                return new ServiceResult<bool>
                {
                    Status = ServiceResultStatus.Error,
                    Message = "An unexpected database error occurred during deletion. Please try again later."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unforeseen error occurred in UserService.Delete for Id: {Id}.", id);
            return new ServiceResult<bool>
            {
                Status = ServiceResultStatus.Error,
                Message = "An unexpected server error occurred during deletion."
            };
        }
    }
}
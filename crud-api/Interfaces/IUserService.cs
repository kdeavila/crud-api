using crud_api.Common;
using crud_api.DTOs.Common;
using crud_api.DTOs.User;

namespace crud_api.Interfaces;

public interface IUserService
{
    Task<ServiceResult<PaginatedResultDto<UserGetDto>>> GetAllPaginatedAndFiltered(
        UserQueryParamsDto userQueryParamsDto);

    Task<ServiceResult<UserGetDto>> GetById(int id);
    Task<ServiceResult<UserGetDto>> Update(int id, UserUpdateDto userUpdateDto);
    Task<ServiceResult<bool>> Delete(int id);
}
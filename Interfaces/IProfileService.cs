using crud_api.Common;
using crud_api.DTOs.Common;
using crud_api.DTOs.Profile;

namespace crud_api.Interfaces;

public interface IProfileService
{
    Task<ServiceResult<PaginatedResultDto<ProfileGetDto>>> GetAllPaginatedAndFiltered(
        ProfileQueryParams profileQueryParams);
    Task<ServiceResult<ProfileGetDto>> GetById(int id);
    Task<ServiceResult<ProfileGetDto>> Create(ProfileCreateDto profileCreateDto);

    Task<ServiceResult<ProfileGetDto>> Update(int id, ProfileUpdateDto profileUpdateDto);
    Task<ServiceResult<bool>> Delete(int id);
}
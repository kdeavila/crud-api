using crud_api.Common;
using crud_api.DTOs.Common;
using crud_api.DTOs.Employee;

namespace crud_api.Interfaces;

public interface IEmployeeService
{
    Task<ServiceResult<PaginatedResultDto<EmployeeGetDto>>> GetAllPaginatedAndFiltered(
        EmployeeQueryParamsDto employeeQueryParamsDto);

    Task<ServiceResult<EmployeeGetDto>> GetById(int id);

    Task<ServiceResult<EmployeeGetDto>> Create(EmployeeCreateDto employeeCreateDto);
    Task<ServiceResult<EmployeeGetDto>> Update(int id, EmployeeUpdateDto employeeUpdateDto);
    Task<ServiceResult<bool>> Delete(int id);
}
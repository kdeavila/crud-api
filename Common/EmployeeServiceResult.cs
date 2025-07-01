using crud_api.DTOs;

namespace crud_api.Common;

public class EmployeeServiceResult
{
    public ServiceResultStatus Status { get; set; }
    public EmployeeDto? Data { get; set; }
    public string? Message { get; set; }
}
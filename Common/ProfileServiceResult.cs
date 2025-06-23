using crud_api.DTOs;

namespace crud_api.Common;

public class ProfileServiceResult
{
    public ServiceResultStatus Status { get; set; }
    public ProfileDto? Data { get; set; }
    public string? Message { get; set; }
}
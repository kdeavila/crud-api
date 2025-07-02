using crud_api.Common;

namespace crud_api.DTOs;

public class UserQueryParamsDto
{
    public string? Email { get; set; }
    public UserRole? Role { get; set; }
    public QueryParamsDto QueryParams { get; set; } = new QueryParamsDto();
}

using crud_api.Common;
using crud_api.DTOs.Common;

namespace crud_api.DTOs.User;

public class UserQueryParamsDto
{
    public string? Email { get; set; }
    public UserRole? Role { get; set; }
    public QueryParamsDto QueryParams { get; set; } = new QueryParamsDto();
}
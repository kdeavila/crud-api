using crud_api.Common;

namespace crud_api.DTOs.User;

public class UserGetDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
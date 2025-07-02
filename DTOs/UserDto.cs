using crud_api.Common;

namespace crud_api.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
}
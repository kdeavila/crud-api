using System.ComponentModel.DataAnnotations;
using crud_api.Common;
using crud_api.Common.ValidationAttributes;

namespace crud_api.DTOs.User;

public class UserUpdateDto
{
    [Required(ErrorMessage = "Id is required for update")]
    public int Id { get; set; }

    [EmailAddress(ErrorMessage = "Email is invalid")]
    public string? Email { get; set; }

    [IsValidEnum(ErrorMessage = "Role must be a valid UserRole (Viewer, Manager, Admin).")]
    public UserRole? Role { get; set; }
}
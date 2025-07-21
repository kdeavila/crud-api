using System.ComponentModel.DataAnnotations;
using crud_api.Common;
using crud_api.Common.ValidationAttributes;

namespace crud_api.DTOs.Auth;

public class UserRegisterDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public required string Password { get; set; }

    [IsValidEnum(ErrorMessage = "Role must be a valid UserRole (Viewer, Manager, Admin).")]
    public UserRole Role { get; set; } = UserRole.Viewer;
}
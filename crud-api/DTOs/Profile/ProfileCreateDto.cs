using System.ComponentModel.DataAnnotations;

namespace crud_api.DTOs.Profile;

public class ProfileCreateDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "The name must be maximum 100 characters")]
    public required string Name { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace crud_api.DTOs.Profile;

public class ProfileUpdateDto
{
    [Required(ErrorMessage = "Id is required for update")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "The name must be maximum 100 characters")]
    public required string Name { get; set; }
}
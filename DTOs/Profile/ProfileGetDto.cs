using System.ComponentModel.DataAnnotations;

namespace crud_api.DTOs.Profile;

public class ProfileGetDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
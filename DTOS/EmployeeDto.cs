using System.ComponentModel.DataAnnotations;

namespace crud_api.DTOS;

public class EmployeeDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "The name must be maximum 100 characters")]
    public required string FullName { get; set; }

    [Range(0, 10000000, ErrorMessage = "Salary must be between 0 and 10000000")]
    public int Salary { get; set; }

    [Required(ErrorMessage = "IdProfile is required")]
    public int IdProfile { get; set; }

    public string? NameProfile { get; set; }
}
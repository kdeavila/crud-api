using System.ComponentModel.DataAnnotations;

namespace crud_api.DTOs.Employee;

public class EmployeeUpdateDto
{
    [Required(ErrorMessage = "Id is required for update")]
    public int Id { get; set; }

    [StringLength(100, ErrorMessage = "The name must be maximum 100 characters")]
    public string? FullName { get; set; }

    [Range(0, 10000000, ErrorMessage = "Salary must be between 0 and 10000000")]
    public decimal? Salary { get; set; }

    public int? IdProfile { get; set; }
}
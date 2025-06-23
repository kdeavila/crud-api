using System.ComponentModel.DataAnnotations;

namespace crud_api.Entities;

public class Profile
{
    public int Id { get; init; }
    [Required]
    [MaxLength(100)]
    public required string Name { get; init; }
    public virtual ICollection<Employee> EmployeesReference { get; set; }
}
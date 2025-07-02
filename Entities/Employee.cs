using System.ComponentModel.DataAnnotations;

namespace crud_api.Entities;

public class Employee
{
    public int Id { get; init; }
    [Required]
    [MaxLength(100)]
    public required string FullName { get; set; }
    public decimal Salary { get; set; }
    public int IdProfile  { get; set; }
    public virtual Profile? ProfileReference { get; init; }
}
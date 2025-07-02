namespace crud_api.DTOs.Employee;

public class EmployeeGetDto
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public decimal Salary { get; set; }
    public int IdProfile { get; set; }
    public string? NameProfile { get; set; }
}
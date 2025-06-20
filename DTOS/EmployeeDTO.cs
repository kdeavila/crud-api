namespace crud_api.DTOS;

public class EmployeeDTO
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public int Salary { get; set; }
    public int IdProfile  { get; set; }
    public string? NameProfile { get; set; }
}


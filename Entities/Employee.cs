namespace crud_api.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Salary { get; set; }
    public int IdProfile  { get; set; }
    public virtual Profile ProfileReference { get; set; }
}
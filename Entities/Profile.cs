namespace crud_api.Entities;

public class Profile
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Employee> EmployeesReference { get; set; }
}
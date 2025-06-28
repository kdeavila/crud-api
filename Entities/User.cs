namespace crud_api.Entities;

public class User
{
    public int Id { get; init; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? Role { get; set; }
}
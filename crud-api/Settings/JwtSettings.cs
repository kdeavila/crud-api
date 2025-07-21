namespace crud_api.Settings;

public class JwtSettings
{
    public required string SecretKey { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int ExpirationMinutes { get; set; }
}
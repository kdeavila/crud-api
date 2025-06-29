namespace crud_api.Common;

public class AuthServiceResult
{
    public ServiceResultStatus Status { get; set; }
    public string? JWT { get; set; }
    public string? Message { get; set; }
}
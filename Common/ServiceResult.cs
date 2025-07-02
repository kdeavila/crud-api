namespace crud_api.Common;

public class ServiceResult<T>
{
    public ServiceResultStatus Status { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
}
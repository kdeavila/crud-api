namespace crud_api.DTOs.Common;

public class ErrorResponseDto
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
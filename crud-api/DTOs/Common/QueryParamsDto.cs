namespace crud_api.DTOs.Common;

public class QueryParamsDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    public string? SortBy { get; set; } = "id";
    public string? Order { get; set; } = "asc";
}
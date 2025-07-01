namespace crud_api.DTOs;

public class EmployeeQueryParamsDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    public string? FullName { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public int? IdProfile { get; set; }

    public string? SortBy { get; set; } = "id";
    public string? Order { get; set; } = "asc";
}
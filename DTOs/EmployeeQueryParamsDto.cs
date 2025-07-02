namespace crud_api.DTOs;

public class EmployeeQueryParamsDto
{
    public string? FullName { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public int? IdProfile { get; set; }

    public QueryParamsDto QueryParams { get; set; } = new QueryParamsDto();
}
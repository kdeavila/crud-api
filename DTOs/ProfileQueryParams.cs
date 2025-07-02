namespace crud_api.DTOs;

public class ProfileQueryParams
{
    public string? Name { get; set; }
    public QueryParamsDto QueryParams { get; set; } = new QueryParamsDto();
}
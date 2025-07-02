using crud_api.DTOs.Common;

namespace crud_api.DTOs.Profile;

public class ProfileQueryParams
{
    public string? Name { get; set; }
    public QueryParamsDto QueryParams { get; set; } = new QueryParamsDto();
}
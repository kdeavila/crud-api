using crud_api.Common;

namespace crud_api.DTOs;

public class PaginatedResultDto<T>
{
    public int TotalCount { get; set;  }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public List<T>? Data { get; set; }
}
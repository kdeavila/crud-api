namespace crud_api.Common;

public enum ServiceResultStatus
{
    Success,
    Created, 
    NotFound,
    InvalidInput,
    Unauthorized,
    Forbidden,
    Conflict,
    Error,
    Deleted
}
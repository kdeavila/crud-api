using crud_api.Common;
using crud_api.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class BaseController : ControllerBase
{
    protected IActionResult ProcessServiceResult<T>(ServiceResult<T> serviceResult)
    {
        return serviceResult.Status switch
        {
            ServiceResultStatus.Success => Ok(serviceResult.Data),
            ServiceResultStatus.Created => StatusCode(201, serviceResult.Data),
            ServiceResultStatus.Deleted => NoContent(),

            ServiceResultStatus.InvalidInput => BadRequest(new ErrorResponseDto
                { Status = serviceResult.Status.ToString(), Message = serviceResult.Message }),
            ServiceResultStatus.NotFound => NotFound(new ErrorResponseDto
                { Status = serviceResult.Status.ToString(), Message = serviceResult.Message }),
            ServiceResultStatus.Conflict => Conflict(new ErrorResponseDto
                { Status = serviceResult.Status.ToString(), Message = serviceResult.Message }),
            ServiceResultStatus.Unauthorized => Unauthorized(new ErrorResponseDto
                { Status = serviceResult.Status.ToString(), Message = serviceResult.Message }),
            
            ServiceResultStatus.Forbidden => Forbid(),

            _ => StatusCode(500,
                new ErrorResponseDto
                    { Status = nameof(ServiceResultStatus.Error), Message = "An unexpected server error occurred." })
        };
    }

    protected IActionResult ProcessServiceResultForCreate<T>(ServiceResult<T> serviceResult, string actionName,
        object routeValues)
    {
        return serviceResult.Status switch
        {
            ServiceResultStatus.Created => CreatedAtAction(actionName, routeValues, serviceResult.Data),
            _ => ProcessServiceResult(serviceResult)
        };
    }
}
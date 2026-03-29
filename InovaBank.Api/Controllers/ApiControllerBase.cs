using InovaBank.Domain.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace InovaBank.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.StatusCode switch
            {
                201 => Created(string.Empty, new ApiResponse<T> { Success = true, Data = result.Value }),
                204 => NoContent(),
                _ => Ok(new ApiResponse<T> { Success = true, Data = result.Value })
            };
        }

        var errorResponse = new ApiResponse<T>
        {
            Success = false,
            Error = result.Error ?? "Erro não especificado."
        };

        return StatusCode(result.StatusCode, errorResponse);
    }
}

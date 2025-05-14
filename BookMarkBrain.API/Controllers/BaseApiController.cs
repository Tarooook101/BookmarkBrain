using BookMarkBrain.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleResult<T>(ServiceResult<T> result)
    {
        if (result == null)
            return NotFound();

        if (result.Success && result.Data != null)
            return Ok(result);

        if (result.Success)
            return NoContent();

        if (result.Errors != null && result.Errors.Count > 0)
            return BadRequest(result);

        return StatusCode(500, result);
    }
}
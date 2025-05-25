using CW_9_s31552.Exceptions;
using CW_9_s31552.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW_9_s31552.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NfzController(IDbService service) : ControllerBase
{

    [HttpGet]
    [Route("patients/{id:int}")]
    public async Task<IActionResult> GetPatientsAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.GetPatientWithDetailsAsync(id, cancellationToken));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
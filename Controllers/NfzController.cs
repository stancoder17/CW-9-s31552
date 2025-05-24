using CW_9_s31552.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CW_9_s31552.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NfzController : ControllerBase
{
    private readonly NfzDbContext _dbContext;
    
    public NfzController(NfzDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("patients")]
    public async Task<IActionResult> GetPatientsAsync(CancellationToken cancellationToken)
    {
        // open, select * from patients -> Patients, close
        var data = await _dbContext.Patients
            .Include(p => p.Prescriptions)
            .ToListAsync(cancellationToken);
        return Ok(data);
    }
}
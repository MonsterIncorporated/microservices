using Microsoft.AspNetCore.Mvc;

namespace NumberGenerator.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController(HealthService healthService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        var healthy = await healthService.GetHealthAsync();
        if (healthy)
        {
            return Ok(new { status = "Healthy" });
        }
        else
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { status = "Unhealthy" });
        }
        
    }
}

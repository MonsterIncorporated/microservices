using Microsoft.AspNetCore.Mvc;

namespace NumberGenerator.Controllers;

[ApiController]
[Route("[controller]")]
public class NumberController : ControllerBase
{
    [HttpGet]
    public int Get()
    {
        return 67;
    }
}

using Microsoft.AspNetCore.Mvc;

namespace NumberGenerator.Controllers;

[ApiController]
[Route("[controller]")]
public class NumberController : ControllerBase
{
    [HttpGet]
    public int Get(int min, int max)
    {
        var random = new Random().Next(min, max + 1);
        return random;
    }
}

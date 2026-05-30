using Microsoft.AspNetCore.Mvc;

namespace NumberGenerator.Controllers;

[ApiController]
[Route("[controller]")]
public class NumberController(INumberService numberService) : ControllerBase
{
    [HttpGet]
    public async Task<ICollection<GeneratedNumberDto>> GetNumbers(Guid userId)
    {
        var numbers = await numberService.GetNumbers(userId);
        return numbers;
    }

    [HttpPost]
    public async Task<GeneratedNumberDto> CreateGeneratedNumberDto([FromBody] CreateGeneratedNumberDto createGeneratedNumberDto)
    {
        var generatedNumberDto = await numberService.CreateGeneratedNumberAsync(createGeneratedNumberDto);
        return generatedNumberDto;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGeneratedNumberAsync(Guid id)
    {
        await numberService.DeleteGeneratedNumberAsync(id);
        return NoContent();
    }
}

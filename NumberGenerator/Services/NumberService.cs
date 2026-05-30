using Microsoft.EntityFrameworkCore;

public class NumberService(ILogger<NumberService> logger, AppDbContext dbContext) : INumberService
{
    public async Task<ICollection<GeneratedNumberDto>> GetNumbers(Guid userId)
    {
        logger.LogInformation("Retrieving numbers for user {UserId}", userId);
        var generatedNumbers = await dbContext.GeneratedNumbers.Where(gn => gn.UserId == userId).ToListAsync();
        logger.LogInformation("Retrieved {Count} numbers for user {UserId}", generatedNumbers.Count, userId);
        
        return generatedNumbers.Select(e => GeneratedNumberDto.toDto(e)).ToList();
    }

    public async Task<GeneratedNumberDto> CreateGeneratedNumberAsync(CreateGeneratedNumberDto createGeneratedNumberDto)
    {
        logger.LogInformation("Creating number for user {UserId} with range {Min}-{Max}", createGeneratedNumberDto.UserId, createGeneratedNumberDto.Min, createGeneratedNumberDto.Max);
        var generatedNumber = new GeneratedNumber
        {
            Id = Guid.NewGuid(),
            UserId = createGeneratedNumberDto.UserId,
            Value = new Random().Next(createGeneratedNumberDto.Min, createGeneratedNumberDto.Max + 1),
            GeneratedAt = DateTime.UtcNow
        };
        dbContext.GeneratedNumbers.Add(generatedNumber);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Created number {Value} for user {UserId}", generatedNumber.Value, createGeneratedNumberDto.UserId);

        return GeneratedNumberDto.toDto(generatedNumber);
    }

    public async Task DeleteGeneratedNumberAsync(Guid numberId)
    {
        logger.LogInformation("Deleting number with id {numberId}", numberId);
        var generatedNumber = await dbContext.GeneratedNumbers.FindAsync(numberId);
        if (generatedNumber != null)
        {
            dbContext.GeneratedNumbers.Remove(generatedNumber);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Deleted number with id {numberId}", numberId);
        }
        else
        {
            logger.LogWarning("Number with id {numberId} not found for deletion", numberId);
        }
    }
}
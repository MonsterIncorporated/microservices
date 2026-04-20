public class NumberService(ILogger<NumberService> logger)
{
    public int? GetNumber()
    {
        logger.LogInformation("GetNumber called");
        return 67;
    }
}
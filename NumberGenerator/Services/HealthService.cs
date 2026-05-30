using NumberGenerator.Services;

public class HealthService(ILogger<HealthService> logger)
{
    public async Task<bool> GetHealthAsync()
    {
        logger.LogInformation("Health check called");
        return true;
    }
}
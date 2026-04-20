using NumberGenerator.Services;

public class HealthService(ILogger<HealthService> logger, NumberService numberService, MqHelperService mqHelperService)
{
    public async Task<bool> GetHealthAsync()
    {
        logger.LogInformation("Health check called");

        var number = numberService.GetNumber();
        var helperService = mqHelperService.IsOk();

        if (number != null && helperService)
        {
            return true;
        }
        return false;
    }
}
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace NumberGenerator.Services;

public class RabbitMqService(ILogger<RabbitMqService> logger, MqHelperService mqHelperService, IConfiguration configuration, IServiceScopeFactory scopeFactory) : IHostedService
{
    private AsyncEventingBasicConsumer? consumer;
    public async Task StartAsync(CancellationToken cancellationToken)
    {   
        logger.LogInformation("Starting RabbitMQService");
        var user = configuration.GetValue<string>("RABBIT_USER");
        var pass = configuration.GetValue<string>("RABBIT_PASS");
        var host = configuration.GetValue<string>("RABBIT_HOST");

        if (user == null || pass == null || host == null)
        {
            logger.LogError("RabbitMQ credentials are not set in environment variables.");
            return;
        }

        consumer = await mqHelperService.StartAsync("numbergenerator", host, user, pass, HandleMessage);

        logger.LogInformation("Started RabbitMQService");
    }

    /**
    * Stops the RabbitMQ service.
    * @param cancellationToken A token to monitor for cancellation requests.
    */
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping RabbitMQService");
        await mqHelperService.StopAsync();
        logger.LogInformation("Stopped RabbitMQService");
    }

    private async Task HandleMessage(Object? ch, BasicDeliverEventArgs eventArgs)
    {
        logger.LogInformation("Handling Message");
        var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

        try
        {
            var transaction = JsonSerializer.Deserialize<TransactionDto>(
                message,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                }
            );

            if (transaction == null)
            {
                logger.LogError("Message not with type Transaction");
                await mqHelperService.RejectMessageAsync(eventArgs.DeliveryTag, false);
                return;
            }

            logger.LogInformation("tId_{id}: Processing message", transaction.TransactionId);
            var result = await TransactionToService(transaction);

            if (result)
            {
                logger.LogInformation("Handling Message " + transaction.TransactionId + " was succesfull");

                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };
                var successMessage = new JsonObject{
                    ["transactionId"] = transaction.TransactionId
                };

                await mqHelperService.PublishSuccessAsync(successMessage, "number.generated", properties);
                await mqHelperService.AcknowledgeMessageAsync(eventArgs.DeliveryTag);
            }
            else
            {
                logger.LogError("Handling Message " + transaction.TransactionId);
                await mqHelperService.RejectMessageAsync(eventArgs.DeliveryTag, false);
            }
        }
        catch (Exception e) 
        {
            logger.LogError("Handling of Message Failed" + e);
            await mqHelperService.RejectMessageAsync(eventArgs.DeliveryTag, false);
        }  
    }

    /**
    * Routes the incoming message to the appropriate service based on the specified type.
    * @param type The type of the message that determines which service to route to.
    * @param message The content of the message to be processed.
    * @param id The transaction ID associated with the message for tracking purposes.
    * @return A boolean value indicating whether the message was successfully processed and routed to the appropriate service.
    * true if the message was successfully processed or the fault was handled; false if error was caused by the service and the message should be requeued.
    */
    private async Task<bool> TransactionToService(TransactionDto transaction)
    {
        logger.LogInformation("Generating Number for Tid_{id}", transaction.TransactionId);
        using var scope = scopeFactory.CreateScope();
        var numberService = scope.ServiceProvider.GetRequiredService<INumberService>();
        await numberService.CreateGeneratedNumberAsync(new CreateGeneratedNumberDto
        {
            UserId = transaction.UserId,
            Min = (int)Math.Pow(10, transaction.NumberOfDigits - 1),
            Max = (int)Math.Pow(10, transaction.NumberOfDigits) - 1
        });
        return true;
    }
}

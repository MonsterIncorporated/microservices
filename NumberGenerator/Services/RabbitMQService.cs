using System.Text;
using System.Text.Json.Nodes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace NumberGenerator.Services;

public class RabbitMqService(ILogger<RabbitMqService> logger, MqHelperService mqHelperService, IConfiguration configuration) : IHostedService
{
    private AsyncEventingBasicConsumer? consumer;
    public async Task StartAsync(CancellationToken cancellationToken)
    {   
        var user = configuration.GetValue<string>("RABBIT_USER");
        var pass = configuration.GetValue<string>("RABBIT_PASS");
        var host = configuration.GetValue<string>("RABBIT_HOST");

        if (user == null || pass == null || host == null)
        {
            logger.LogError("RabbitMQ credentials are not set in environment variables.");
            return;
        }

        consumer = await mqHelperService.StartAsync("numbergenerator", host, user, pass);
        consumer.ReceivedAsync += async (ch, ea) => await HandleMessage(ch,ea);
    }

    /**
    * Stops the RabbitMQ service.
    * @param cancellationToken A token to monitor for cancellation requests.
    */
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await mqHelperService.StopAsync();
    }

    private async Task HandleMessage(Object? ch, BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
            var header = eventArgs.BasicProperties.Headers;
            var message = Encoding.UTF8.GetString(body);

            if (header == null)
            {
                logger.LogError("Message without headers");
                await mqHelperService.RejectMessageAsync(eventArgs.DeliveryTag, false);
                return;
            }

            var type = mqHelperService.GetKeyFromDictionaryInString(header, "type");
            var id = mqHelperService.GetKeyFromDictionaryInString(header, "transactionId");

            if (type != null && id != null)
            {
                logger.LogInformation("tId_{id}: Processing message", id);
                var result = await TypeToService(type, message, id);

                if (result)
                {
                    await mqHelperService.AcknowledgeMessageAsync(eventArgs.DeliveryTag);

                    var properties = new BasicProperties
                    {
                        ContentType = "application/json",
                        DeliveryMode = DeliveryModes.Persistent
                    };
                    var successMessage = new JsonObject{
                        ["transactionId"] = id
                    };

                    await mqHelperService.PublishSuccessAsync(successMessage, "/numbergenerator", properties);
                }
                else
                {
                    await mqHelperService.RejectMessageAsync(eventArgs.DeliveryTag, false);
                }
            }
            else
            {
                logger.LogError("Message without 'type' and or 'id' header.");
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
    private async Task<bool> TypeToService(string type, string message, string id)
    {
        logger.LogInformation("tId_{id}: Operation of type: {type} is being processed", id, type);
        switch (type)
        {
            case "get":
                //await numberService.GetNumbers(Guid.Parse(message));
                return true;
            default:
                logger.LogError("tId_{id}: Unknown message type: {type}", id, type);

                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };

                var errorMessage = new JsonObject{
                    ["description"] = $"Unknown message type: {type}",
                    ["transactionId"] = id
                };

                await mqHelperService.PublishErrorAsync(errorMessage, "/numbergenerator", properties);
                return false;
        }
    }
}

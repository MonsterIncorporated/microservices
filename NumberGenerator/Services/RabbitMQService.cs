using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json.Nodes;
namespace NumberGenerator.Services;

public class RabbitMqService(NumberService numberService, ILogger<RabbitMqService> logger, MqHelperService mqHelperService) : IHostedService
{
    private AsyncEventingBasicConsumer? consumer;
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        consumer = await mqHelperService.StartAsync("test","localhost","admin","password");

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
                logger.LogError("Message without headers.");
                await mqHelperService.RejectMessageAsync(eventArgs.DeliveryTag, false);
                return;
            }

            var type = mqHelperService.GetKeyFromDictionaryInString(header, "type");
            var id = mqHelperService.GetKeyFromDictionaryInString(header, "transactionId");

            if (type != null && id != null)
            {
                logger.LogInformation("Processing message transactionId: {transactionId}", id);
                var result = await TypeToService(type, message, id);

                if (result)
                {
                    await mqHelperService.AcknowledgeMessageAsync(eventArgs.DeliveryTag);
                }
                else
                {
                    await mqHelperService.RejectMessageAsync(eventArgs.DeliveryTag, true);
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
        switch (type)
        {
            case "get":
                numberService.GetNumber();
                return true;
            default:
                Console.WriteLine($"Unknown message type: {type}");

                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };

                var errorMessage = new JsonObject{
                    ["code"] = "404",
                    ["title"] = "unknown type",
                    ["description"] = $"Unknown message type: {type}",
                    ["transactionId"] = id
                };

                await mqHelperService.PublishErrorAsync(errorMessage, "/numbergenerator", properties);
                return false;
        }
    }
}

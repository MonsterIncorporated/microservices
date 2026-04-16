using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json.Nodes;
namespace NumberGenerator.Services;

public class MqHelperService(ILogger<MqHelperService> logger)
{
    private Guid id = Guid.NewGuid();
    private IChannel? channel;
    private IConnection? connection;
    private AsyncEventingBasicConsumer? consumer;

    /**
    * Starts the RabbitMQ service by establishing a connection to the RabbitMQ server, creating a channel, and setting up a consumer for the specified queue.
    * @param queueName The name of the RabbitMQ queue to consume messages from.
    * @param hostname The hostname of the RabbitMQ server to connect to.
    * @param username The username for authenticating with the RabbitMQ server.
    * @param password The password for authenticating with the RabbitMQ server.
    * @return An instance of AsyncEventingBasicConsumer that can be used to handle incoming messages from the specified queue.
    */
    public async Task<AsyncEventingBasicConsumer> StartAsync(string queueName, string hostname, string username, string password)
    {
        logger.LogInformation("{id}: Starting RabbitMQ service and connecting to queue: {queueName} at {hostname}", id, queueName, hostname);
        var factory = new ConnectionFactory
        { 
            HostName = hostname, 
            UserName = username, 
            Password = password 
        };

        connection = await factory.CreateConnectionAsync();
        channel = await connection.CreateChannelAsync();
        consumer = new AsyncEventingBasicConsumer(channel);

        await channel.BasicConsumeAsync(queueName, false, consumer);

        logger.LogInformation("{id}: RabbitMQ service started and consuming from queue: {queueName}", id, queueName);
        return consumer;
    }

    /**
    * Stops the RabbitMQ service by closing the channel and connection if they are open.
    */
    public async Task StopAsync()
    {
        logger.LogInformation("{id}: Stopping RabbitMQ service and closing connection", id);
        if (channel != null && connection != null)
        {
            await channel.CloseAsync();
            await connection.CloseAsync();
            await channel.DisposeAsync();
            await connection.DisposeAsync();
        }
        logger.LogInformation("{id}: RabbitMQ service stopped and connection closed", id);
    }

    /**
    * Retrieves a string value from a dictionary based on the specified key.
    *
    * @param dictionary The dictionary containing the key-value pairs.
    * @param key The key for which to retrieve the value from the dictionary.   
    * @return The string value associated with the specified key, or null if the key is not found.
    */
    public string? GetKeyFromDictionaryInString(IDictionary<string, object?> dictionary, string key)
    {
        if (dictionary.TryGetValue(key, out var idVal) && idVal is byte[] idBytes)
            return Encoding.UTF8.GetString(idBytes);
        else
            return null;
    }

    /**
    * Publishes an error message to the "fail" exchange with the specified routing key and properties.
    * 
    * @param rabbitMqService The instance of RabbitMqService to access the channel for publishing the error.
    * @param errorMessage The error message to be published, represented as a JsonObject.
    * @param routingKey The routing key to be used for publishing the message.
    * @param properties The basic properties to be associated with the message, such as content type and delivery mode.
    */
    public async Task PublishErrorAsync(JsonObject errorMessage, string routingKey, BasicProperties properties){
        if (channel != null)
        {
            await channel.BasicPublishAsync(exchange: "fail", routingKey: routingKey, mandatory: true, basicProperties: properties, body: Encoding.UTF8.GetBytes(errorMessage.ToString()));
        }
    }

    /**
    * Publishes a success message to the "success" exchange with the specified routing key and properties.
    *
    * @param rabbitMqService The instance of RabbitMqService to access the channel for publishing the message.
    * @param message The success message to be published, represented as a JsonObject.
    * @param routingKey The routing key to be used for publishing the message.
    * @param properties The basic properties to be associated with the message, such as content type and delivery mode.
    */
    public async Task PublishSuccessAsync(JsonObject message, string routingKey, BasicProperties properties){
        if (channel != null)
        {
            await channel.BasicPublishAsync(exchange: "success", routingKey: routingKey, mandatory: true, basicProperties: properties, body: Encoding.UTF8.GetBytes(message.ToString()));
        }
    }

    /**
    * Acknowledges the successful processing of a message by sending an acknowledgment to the RabbitMQ server.
    * @param deliveryTag The delivery tag of the message to be acknowledged, which is a unique identifier for the message within the channel.
    */
    public async Task AcknowledgeMessageAsync(ulong deliveryTag)
    {
        if (channel != null)
        {
            await channel.BasicAckAsync(deliveryTag, false);
        }
    }

    /**
    * Rejects a message that could not be processed successfully, with an option to requeue the message for later processing.
    * @param deliveryTag The delivery tag of the message to be rejected.
    * @param requeue A boolean value indicating whether the rejected message should be requeued for
    * later processing (true) or discarded (false).
    */
    public async Task RejectMessageAsync(ulong deliveryTag, bool requeue)
    {
        if (channel != null)
        {
            await channel.BasicNackAsync(deliveryTag, false, requeue);
        }
    }
}

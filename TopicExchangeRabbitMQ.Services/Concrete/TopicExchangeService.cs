using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.Common;
using System.Text;
using System.Threading.Channels;
using TopicExchangeRabbitMQ.Services.Abstract;

namespace TopicExchangeRabbitMQ.Services.Concrete;
public class TopicExchangeService : ITopicExchangeService
{
    private readonly ConnectionFactory _connectionFactory;
    private const string ExchangeName = "topic_exchange";
    private const string RabbitMQUri = "amqps://zuduguoa:i3rDng9dw_Zfoa29DkCTQPr04FeDoexR@ostrich.lmq.cloudamqp.com/zuduguoa";
    private readonly List<string> _receivedMessages = new();

    public TopicExchangeService()
    {
        _connectionFactory = new ConnectionFactory { Uri = new Uri(RabbitMQUri) };
    }

    public async Task SendMessageToTopicExchangeAsync(string routingKey, string message)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // Declare the exchange
        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic,durable:true,autoDelete:false);

        // Publish the message
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: ExchangeName, routingKey: routingKey, body: body);

        await Console.Out.WriteLineAsync($"Message '{message}' sent to routing key '{routingKey}'.");
    }

    public async Task ReceiveMessageFromQueueAsync(string queueName, List<string> routingKeys)
    {
        try
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            // Declare the exchange and queue
            await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic, durable: true);
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            // Bind the queue to the exchange for each routing key
            foreach (var routingKey in routingKeys)
            {
                await channel.QueueBindAsync(queue: queueName, exchange: ExchangeName, routingKey: routingKey);
            }

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync +=  async (model, ea) =>
            {
                // Check if the message was sent to one of the specified routing keys
                if (routingKeys.Contains(ea.RoutingKey))
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    // Add to received messages
                    lock (_receivedMessages)
                    {
                        _receivedMessages.Add(message);
                    }

                    Console.WriteLine($"Received message with routing key '{ea.RoutingKey}': {message}");
                }
                else
                {
                     Console.WriteLine($"Ignored message with routing key '{ea.RoutingKey}'");
                }

                await Task.CompletedTask; // Placeholder for async operations
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

            await Console.Out.WriteLineAsync($"Waiting for messages on queue '{queueName}'...");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving messages: {ex.Message}");
        }
    }


    public List<string> GetReceivedMessages()
    {
        return new List<string>(_receivedMessages); // Return a copy to ensure thread safety
    }

    private Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
    {
        var body = @event.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        _receivedMessages.Add(message); // Store received messages
        Console.WriteLine($"Received message: {message}");
        return Task.CompletedTask;
    }
}

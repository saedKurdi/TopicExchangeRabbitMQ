namespace TopicExchangeRabbitMQ.Services.Abstract;
public interface ITopicExchangeService
{
    /// <summary>
    /// Sends a message to a specific topic exchange with a routing key.
    /// </summary>
    /// <param name="routingKey">The routing key for the message.</param>
    /// <param name="message">The message to send.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task SendMessageToTopicExchangeAsync(string routingKey, string message);

    /// <summary>
    /// Receives messages from a specified queue and processes them.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task ReceiveMessageFromQueueAsync(string queueName, List<string> routingKeys);

    List<string> GetReceivedMessages();
}
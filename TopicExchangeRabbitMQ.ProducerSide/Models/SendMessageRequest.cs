namespace TopicExchangeRabbitMQ.ProducerSide.Models;
public class SendMessageRequest
{
    public List<string>? Groups { get; set; }
    public string? Message { get; set; }
}


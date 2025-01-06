namespace TopicExchangeRabbitMQ.Models.Models;
public class TopicExchangeGroup
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> SubGroups { get; set; } = new List<string>();
}

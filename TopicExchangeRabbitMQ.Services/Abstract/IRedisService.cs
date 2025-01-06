using TopicExchangeRabbitMQ.Models.Models;

namespace TopicExchangeRabbitMQ.Business.Abstract;
public interface IRedisService
{
    void WriteGroupToRedis(TopicExchangeGroup group);
    List<string> GetSubGroupsOfGroup(string groupName);
    List<string> GetMainGroups();
}

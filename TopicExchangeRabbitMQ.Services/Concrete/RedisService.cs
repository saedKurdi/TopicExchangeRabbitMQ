using StackExchange.Redis;
using System.Text.Json;
using TopicExchangeRabbitMQ.Business.Abstract;
using TopicExchangeRabbitMQ.Models.Models;

namespace TopicExchangeRabbitMQ.Business.Concrete;
public class RedisService : IRedisService
{
    private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis-15253.c277.us-east-1-3.ec2.redns.redis-cloud.com:15253,password=PRgipOSCveFVseB7vo4GX043N1A2tTCa");

    public void WriteGroupToRedis(TopicExchangeGroup group)
    {
        var db = redis.GetDatabase();
        
        group.Id = Guid.NewGuid().ToString();
        var parentGroupName = string.Empty;
        if (group.Name.Contains("."))
        {
            var nameArray = group.Name.Split('.');
            for(int i = 0; i < nameArray.Length - 1; i++)
            {
                parentGroupName += nameArray[i];
                if (i < nameArray.Length - 2) 
                    parentGroupName += '.';
            }
        }
        else
        {
            parentGroupName = string.Empty;
        }
        if(parentGroupName != string.Empty)
        {
            var parentGroupSubGroups = db.HashGet(string.Concat("Groups:",parentGroupName), "SubGroups");
            if (parentGroupSubGroups != "")
            {
                var subGroups = JsonSerializer.Deserialize<List<string>>(parentGroupSubGroups);
                subGroups?.Add(group.Name);
                db.HashSet(string.Concat("Groups:",parentGroupName), "SubGroups",JsonSerializer.Serialize(subGroups));
            }
            else
            {
                throw new ArgumentNullException("The Parent Group was not found in redis !");
            }
        }
        db.HashSet(string.Concat("Groups:",group.Name), new HashEntry[]
        {
            new HashEntry("Id",Guid.NewGuid().ToString()),
            new HashEntry("Name",group.Name),
            new HashEntry("SubGroups",JsonSerializer.Serialize(group.SubGroups)),
        });
    }

    public List<string> GetSubGroupsOfGroup(string groupName)
    {
        var db = redis.GetDatabase();
        var subGroups = db.HashGet($"Groups:{groupName}", "SubGroups");
        return subGroups.IsNullOrEmpty ? new List<string>() : JsonSerializer.Deserialize<List<string>>(subGroups);
    }

    public List<string> GetMainGroups()
    {
        var server = redis.GetServer("redis-15253.c277.us-east-1-3.ec2.redns.redis-cloud.com:15253"); // Provide host and port
        var keys = server.Keys(pattern: "Groups:*");

        var groupNames = new List<string>();
        foreach (var key in keys)
        {
            if(!key.ToString().Contains('.'))
                groupNames.Add(key.ToString().Replace("Groups:", ""));
        }

        return groupNames;
    }
}

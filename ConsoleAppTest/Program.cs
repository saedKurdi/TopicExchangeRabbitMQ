using TopicExchangeRabbitMQ.Business.Concrete;
using TopicExchangeRabbitMQ.Models.Models;
using TopicExchangeRabbitMQ.Services.Abstract;
using TopicExchangeRabbitMQ.Services.Concrete;

Console.WriteLine("Hello world !");

var redisService = new RedisService();

redisService.WriteGroupToRedis(new TopicExchangeGroup { Name = "sport" });

redisService.WriteGroupToRedis(new TopicExchangeGroup { Name = "programming" });

redisService.WriteGroupToRedis(new TopicExchangeGroup { Name = "sport.basketball" });

redisService.WriteGroupToRedis(new TopicExchangeGroup { Name = "sport.basketball.nba" });

redisService.WriteGroupToRedis(new TopicExchangeGroup { Name = "sport.basketball.turkish-league" });

redisService.WriteGroupToRedis(new TopicExchangeGroup { Name = "programming.c#" });

redisService.WriteGroupToRedis(new TopicExchangeGroup { Name = "programming.c++" });

redisService.WriteGroupToRedis(new TopicExchangeGroup { Name = "programming.c#.oop" });

var list = redisService.GetMainGroups();
foreach (var group in list)
{
    Console.WriteLine(group);
}

//var subGroupsOfSport = redisService.GetSubGroupsOfGroup("sport");
//var subGroupsOfSportBasketball = redisService.GetSubGroupsOfGroup("sport.basketball");
//var subGroupsOfProgramming = redisService.GetSubGroupsOfGroup("programming");
//var subGroupsOfProgrammingCS = redisService.GetSubGroupsOfGroup("programming.c#");

//Console.WriteLine("Sub groups of sport");
//foreach (var item in subGroupsOfSport)
//{
//    Console.WriteLine(item);
//}

//Console.WriteLine("Sub groups of sport.basketball");
//foreach (var item in subGroupsOfSportBasketball)
//{
//    Console.WriteLine(item);
//}

//Console.WriteLine("Sub groups of programming");
//foreach (var item in subGroupsOfProgramming)
//{
//    Console.WriteLine(item);
//}

//Console.WriteLine("Sub groups of programming.c#");
//foreach (var item in subGroupsOfProgrammingCS)
//{
//    Console.WriteLine(item);
//}



//var tes = new TopicExchangeService();

//await tes.SendMessageToTopicExchangeAsync("sport.basketball.nba", "Jordan has ended his career!");

//await tes.SendMessageToTopicExchangeAsync("news.weather", "It's sunny today.");

//await tes.SendMessageToTopicExchangeAsync("programming.c#.oop", "OOP is object oriented programming ");

//var routingKeys = new List<string> { "sport.basketball.nba" , "programming.c#.oop" };

//await tes.ReceiveMessageFromQueueAsync("mytest_queue", routingKeys);
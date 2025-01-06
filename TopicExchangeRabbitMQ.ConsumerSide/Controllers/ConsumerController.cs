using Microsoft.AspNetCore.Mvc;
using TopicExchangeRabbitMQ.Business.Abstract;
using TopicExchangeRabbitMQ.ReceiverSide.Models;
using TopicExchangeRabbitMQ.Services.Abstract;

public class ConsumerController : Controller
{
    private readonly ITopicExchangeService _topicExchangeService;
    private readonly IRedisService _redisService;

    public ConsumerController(ITopicExchangeService topicExchangeService, IRedisService redisService)
    {
        _topicExchangeService = topicExchangeService;
        _redisService = redisService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var mainGroups = _redisService.GetMainGroups();
        var model = new ConsumerViewModel{ GroupNames = mainGroups };
        return View(model);
    }

    [HttpGet]
    public IActionResult GetSubGroupsOfGroup(string groupName)
    {
        var subGroups = _redisService.GetSubGroupsOfGroup(groupName);
        return Ok(subGroups); // Return subgroups as JSON
    }


    [HttpPost]
    public async Task<IActionResult> SubscribeToGroups([FromBody] ConsumerViewModel model)
    {
            var queueName = "CustomTestQueue"; // Generate unique queue name
            await _topicExchangeService.ReceiveMessageFromQueueAsync(queueName, model.GroupNames);
            return Ok("Subscribed successfully to selected groups!");
    }


    [HttpGet]
    public IActionResult GetReceivedMessages()
    {
        var messages = _topicExchangeService.GetReceivedMessages();
        return Ok(messages);
    }
}

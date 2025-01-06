using Microsoft.AspNetCore.Mvc;
using TopicExchangeRabbitMQ.Services.Abstract;
using TopicExchangeRabbitMQ.ProducerSide.Models;
using TopicExchangeRabbitMQ.Business.Abstract;
using TopicExchangeRabbitMQ.Models.Models;

public class ProducerController : Controller
{
    private readonly ITopicExchangeService _topicExchangeService;
    private readonly IRedisService _redisService;

    public ProducerController(ITopicExchangeService topicExchangeService, IRedisService redisService)
    {
        _topicExchangeService = topicExchangeService;
        _redisService = redisService;
    }

    [HttpPost]
    public IActionResult AddGroup([FromBody] GroupNameRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.GroupName))
        {
            return BadRequest("Group name cannot be empty.");
        }

        var newGroup = new TopicExchangeGroup { Name = request.GroupName };
        _redisService.WriteGroupToRedis(newGroup);

        return Ok(newGroup); // Respond with the created group
    }



    public IActionResult Index()
    {
        var groups = _redisService.GetMainGroups();
        var model = new IndexViewModel { GroupNames = groups };
        return View(model);
    }

    [HttpGet]
    public IActionResult GetSubGroupsOfGroup(string groupName)
    {
        var subGroups = _redisService.GetSubGroupsOfGroup(groupName);
        return Ok(subGroups); // Return subgroups as JSON
    }

    [HttpPost]
    public async Task<IActionResult> SendMessageToGroups([FromBody] SendMessageRequest request)
    {
        if (request.Groups == null || !request.Groups.Any())
        {
            return BadRequest("No groups selected.");
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty.");
        }

        foreach (var group in request.Groups)
        {
            await _topicExchangeService.SendMessageToTopicExchangeAsync(group, request.Message);
        }

        return Ok("Message sent successfully to the selected groups.");
    }
}

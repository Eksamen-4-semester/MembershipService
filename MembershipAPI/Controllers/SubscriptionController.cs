using MembershipAPI.Models.DTOs;
using MembershipAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MembershipAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    ILogger<SubscriptionController> _logger;
    ISubscriptionRepository _subscriptionRepository;
    
    public SubscriptionController(
        ILogger<SubscriptionController> logger,
        ISubscriptionRepository subscriptionRepository)
    {
        _logger = logger;
        _subscriptionRepository = subscriptionRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSubscriptions()
    {
        var result = await _subscriptionRepository.GetAllSubscriptionsAsync();
        return Ok(result);
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateSubscription([FromBody] SubscriptionDto subscription)
    {
        if (subscription.Price <= 0)
        {
            return BadRequest("Price must be positive");
        }

        if (string.IsNullOrWhiteSpace(subscription.Name))
        {
            return BadRequest("Name is required");
        }
        
        var created = await _subscriptionRepository.CreateNewSubscriptionAsync(subscription);
        if (!created)
        {
            return StatusCode(500);
        }

        return Created();
    }
    
}
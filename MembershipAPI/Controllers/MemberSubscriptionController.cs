using MembershipAPI.Models.DTOs;
using MembershipAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MembershipAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemberSubscriptionController : ControllerBase
{
    ILogger<MemberSubscriptionController> _logger;
    IMemberSubscriptionRepository _memberSubscriptionRepository;
    
    public MemberSubscriptionController(
        ILogger<MemberSubscriptionController> logger,
        IMemberSubscriptionRepository memberSubscriptionRepository)
    {
        _logger = logger;
        _memberSubscriptionRepository = memberSubscriptionRepository;
    }

    // Membersubscription.TotalPrice skal være den totale pris inkluderet alle 'add-ons'
    [HttpPost]
    public async Task<IActionResult> CreateMemberSubscription([FromBody] MemberSubscriptionDto memberSubscription)
    {
        if (memberSubscription.MemberId <= 0
            || memberSubscription.SubscriptionId <= 0
            || memberSubscription.TotalPrice <= 0)
        {
            return BadRequest("Invalid member id, subscription id or total price");
        }
        
        var res = await _memberSubscriptionRepository.MemberAlreadyHasSubscriptionAsync(memberSubscription.MemberId);
        if (res)
            return Conflict("Member already has a Subscription");
        
        var subscriptionResult = await _memberSubscriptionRepository.CreateMemberSubscriptionAsync(memberSubscription);
        if (!subscriptionResult)
            return BadRequest("Subscription could not be created");
        return Created();
    }
}
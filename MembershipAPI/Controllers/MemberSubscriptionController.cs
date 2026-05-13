using System.Security.Claims;
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

    [Authorize]
    [HttpGet]
    [Route("{memberId:int}")]
    public async Task<IActionResult> GetMemberSubscriptionAsync(int memberId)
    {
        if (memberId <= 0)
            return BadRequest("Invalid member id");
        
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        var isAdminOrTrainer = User.IsInRole("Admin") || User.IsInRole("Trainer");
        
        if (!isAdminOrTrainer && userIdClaim != null && memberId != int.Parse(userIdClaim.Value))
        {
            _logger.LogInformation("Member {MemberId} tried to access subscription for member {TargetMemberId}", userIdClaim.Value, memberId);
            return Forbid();
        }
        
        var result = await _memberSubscriptionRepository.GetMemberSubscriptionByMemberIdAsync(memberId);
        if (result == null)
            return NotFound();
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateMemberSubscription([FromBody] MemberSubscriptionDto memberSubscription)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            _logger.LogError("MemberId claim not found in token");
            return Unauthorized();
        }
        
        var memberId = int.Parse(userIdClaim.Value);
        
        if (memberSubscription.SubscriptionId <= 0)
            return BadRequest("Invalid subscription id");
        
        var res = await _memberSubscriptionRepository.MemberAlreadyHasSubscriptionAsync(memberId);
        if (res)
            return Conflict("Member already has a Subscription");
        
        var subscription = new MemberSubscriptionDto(memberId, memberSubscription.SubscriptionId);
        
        var subscriptionResult = await _memberSubscriptionRepository.CreateMemberSubscriptionAsync(subscription);
        if (!subscriptionResult)
            return BadRequest("Subscription could not be created");
        return Created();
    }
}
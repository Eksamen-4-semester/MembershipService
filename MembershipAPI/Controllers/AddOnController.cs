using MembershipAPI.Models;
using MembershipAPI.Models.DTOs;
using MembershipAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MembershipAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddOnController : ControllerBase
{
    ILogger<AddOnController> _logger;
    IAddOnRepository _addOnRepository;
    IMemberSubscriptionRepository _memberSubscriptionRepository;
    
    public AddOnController(
        ILogger<AddOnController> logger,
        IAddOnRepository repository,
        IMemberSubscriptionRepository memberSubscriptionRepository)
    {
        _logger = logger;
        _addOnRepository = repository;
        _memberSubscriptionRepository = memberSubscriptionRepository;
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateAddOn(AddOnDto addOnDto)
    {
        if (addOnDto.Price <= 0)
        {
            return BadRequest("Price must be greater than 0");
        }

        if (string.IsNullOrWhiteSpace(addOnDto.Name))
        {
            return BadRequest("Name is required");
        }
        
        var result = await _addOnRepository.CreateAddOnAsync(addOnDto);
        if (!result)
        {
            return StatusCode(500);
        }
        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAddOns()
    {
        var result = await _addOnRepository.GetAllAddOnsAsync();
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost]
    [Route("{memberSubscriptionId:int}")]
    public async Task<IActionResult> RegisterAddOnToMemberSubscription(int memberSubscriptionId, [FromBody]SubscriptionAddOnDto addOnDto)
    {
        if (addOnDto.addOnIds == null || addOnDto.addOnIds.Count == 0)
        {
            return BadRequest("No Add ons given");
        }

        if (memberSubscriptionId <= 0)
        {
            return BadRequest("Invalid member subscription id");
        }
        
        var memberSubscription = await _memberSubscriptionRepository.GetMemberSubscriptionByIdAsync(memberSubscriptionId);
        if (memberSubscription == null)
        {
            return BadRequest("Member subscription does not exist");
        }
        
        var addOnsExist = await _addOnRepository.AddOnsExistsAsync(addOnDto.addOnIds);
        if (!addOnsExist)
        {
            return BadRequest("One or more addOns do not exist");
        }

        var result = await _addOnRepository.AssignAddOnsToMemberSubscriptionAsync(memberSubscriptionId, addOnDto.addOnIds);
        if (!result)
        {
            return StatusCode(500);
        }

        return Created();
    }
    
}
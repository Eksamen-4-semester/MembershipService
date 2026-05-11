using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MembershipAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    ILogger<SubscriptionController> _logger;
    public SubscriptionController(ILogger<SubscriptionController> logger)
    {
        _logger = logger;
    }
}
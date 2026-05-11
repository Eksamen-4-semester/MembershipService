using Microsoft.AspNetCore.Mvc;

namespace MembershipAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddOnController : ControllerBase
{
    ILogger<AddOnController> _logger;
    public AddOnController(ILogger<AddOnController> logger)
    {
        _logger = logger;
    }
}
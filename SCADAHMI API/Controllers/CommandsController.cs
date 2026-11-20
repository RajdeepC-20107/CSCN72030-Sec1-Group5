using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCADAHMI_API.Services;
using SCADAHMI_API.Contracts.Responses;
using SCADAHMI_API.Contracts.Requests;

namespace SCADAHMI_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommandsController(CommandDispatcher dispatcher) : ControllerBase
{
    [HttpPost("set-message")]
    public IActionResult SetMessage(SetHelloDeviceMessageRequest request)
    {
        ApiResponse response = dispatcher.Dispatch(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}


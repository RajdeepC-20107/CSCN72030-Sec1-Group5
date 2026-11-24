using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCADAHMI_API.Services;
using SCADAHMI_API.Contracts.Responses;
using SCADAHMI_API.Contracts.Requests;
using Microsoft.AspNetCore.Http.Metadata;

namespace SCADAHMI_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommandsController(CommandDispatcher dispatcher) : ControllerBase
{
    [HttpPost("rename-device")]
    public IActionResult RenameDevice(RenameDeviceRequest request)
        => StandardDispatch(request);

    [HttpPost("set-message")]
    public IActionResult SetMessage(SetHelloDeviceMessageRequest request)
        => StandardDispatch(request);

    [HttpPost("print-messages")]
    public IActionResult PrintMessages(PrintHelloDeviceMessageRequest request)
        => StandardDispatch(request);

    /// <summary>
    /// This method exists since every HttpPost was the same.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>HTTP OK if dispatch succeeds, HTTP BadRequest if not.</returns>
    private IActionResult StandardDispatch(CommandRequest request)
    {
        ApiResponse response = dispatcher.Dispatch(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}


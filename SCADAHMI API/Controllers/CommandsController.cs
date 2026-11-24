using Microsoft.AspNetCore.Mvc;
using SCADAHMI_API.Services;
using SCADAHMI_API.Contracts.Responses;
using SCADAHMI_API.Contracts.Requests;
using SCADAHMI_API.Contracts.Requests.Command;

namespace SCADAHMI_API.Controllers;

/// <summary>
/// The API controller for posting/publishing existing commands.
/// </summary>
/// <param name="dispatcher"></param>
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

    [HttpPost("set-lights-rgb")]
    public IActionResult SetLightsRGB(SetLightsRGBRequest request)
        => StandardDispatch(request);

    /// <summary>
    /// This method exists since every HttpPost was the same.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>HTTP OK if dispatch succeeds, HTTP BadRequest if not.</returns>
    private IActionResult StandardDispatch(ApiRequest request)
    {
        ApiResponse response = dispatcher.Dispatch(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}


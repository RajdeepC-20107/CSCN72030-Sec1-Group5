using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCADAHMI_Project;
using SCADAHMI_API.Contracts.Responses;
using SCADAHMI_API.Contracts.Requests;
using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_API.Services;

namespace SCADAHMI_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DevicesController(
    DeviceRegistry registry,
    DeviceStateNotifier notifier
) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateHelloDeviceRequest request)
    {
        HelloDevice? device = HelloDeviceManager.CreateDevice(
        new CreateHelloDeviceArgs(request.Payload?.Message), registry);

        string requestId = string.IsNullOrWhiteSpace(request.RequestId)
            ? Guid.NewGuid().ToString()
            : request.RequestId;

        if (device is null)
        {
            return BadRequest(new ApiResponse(
                requestId, false, Error: "The device could not be registered."));
        }

        await notifier.PublishAsync(registry, registry.Version);

        return Ok(new ApiResponse(
            requestId,
            true,
            new DeviceStateDto(device.Guid, nameof(HelloDevice), device.Message))
        );
    }

    [HttpGet]
    public IActionResult Get([FromQuery] int since = 0)
    {
        // A client that has never fetched a snapshot (since == 0) always
        // gets the full state; otherwise 304 means "nothing changed".
        if (since > 0 && since >= registry.Version)
            return StatusCode(StatusCodes.Status304NotModified);

        return Ok(DeviceSnapshotFactory.Create(registry, registry.Version));
    }
}


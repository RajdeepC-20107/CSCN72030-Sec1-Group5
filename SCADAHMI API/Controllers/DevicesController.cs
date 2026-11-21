using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCADAHMI_Project;
using SCADAHMI_API.Contracts.Responses;
using SCADAHMI_API.Contracts.Requests.CreateDevice;
using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_API.Services;
using SCADAHMI_API.Contracts.Requests.RemoveDevice;

namespace SCADAHMI_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DevicesController(
    DeviceRegistry registry
) : ControllerBase
{
    [HttpPost("create")]
    public IActionResult Create(CreateDeviceRequest request)
    {
        string requestId = string.IsNullOrWhiteSpace(request.RequestId)
            ? Guid.NewGuid().ToString()
            : request.RequestId;

        // The payload's $type discriminator decides which domain manager runs.
        Device? device = request.Payload switch
        {
            CreateHelloDevicePayload hello => HelloDeviceManager.CreateDevice(
                new CreateHelloDeviceArgs(hello.Message), registry
            ),

            _ => null
        };

        if (device is null)
        {
            return BadRequest(new ApiResponse(
                requestId,
                false,
                Error: "Unsupported or invalid device payload."));
        }

        // Subscribing already bumped the registry version, which raises
        // Changed and pushes a fresh snapshot to all SignalR clients.
        return Ok(new ApiResponse(
            requestId,
            true,
            DeviceStateDtoFactory.Create(device))
        );
    }

    [HttpPost("remove")]
    public IActionResult Remove(RemoveDeviceRequest request)
    {
        string requestId = string.IsNullOrWhiteSpace(request.RequestId)
            ? Guid.NewGuid().ToString()
            : request.RequestId;

        if (request.Payload is null)
        {
            return BadRequest(new ApiResponse(
                requestId, false, Error: "A removal payload is required."));
        }

        // The payload's $type discriminator decides which domain manager runs.
        bool? removed = request.Payload switch
        {
            RemoveHelloDevicePayload hello => HelloDeviceManager.RemoveDevice(hello.Guid, registry),

            _ => null
        };

        if (removed is null)
        {
            return BadRequest(new ApiResponse(
                requestId, 
                false, 
                Error: "Unsupported device payload type."
            ));
        }

        if (removed is false)
        {
            return NotFound(new ApiResponse(
                requestId,
                false,
                Error: $"No device of specified type with GUID '{request.Payload.Guid}' is registered."
            ));
        }

        // Unsubscribing already bumped the registry version, which raises
        // Changed and pushes a fresh snapshot to all SignalR clients.
        return Ok(new ApiResponse(
            requestId,
            true
        ));
    }

    [HttpGet]
    public IActionResult Get([FromQuery] int since = 0)
    {
        // A client that has never fetched a snapshot (since == 0) always
        // gets the full state; otherwise 304 means "nothing changed".
        if (since > 0 && since >= registry.Version) 
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return Ok(DeviceSnapshotFactory.Create(registry, registry.Version));
    }
}


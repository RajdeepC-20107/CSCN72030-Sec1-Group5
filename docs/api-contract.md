# Proposed API contract (SCRUM-18 and SCRUM-19)

A proposal for the data shared between the backend and the Android app. Nothing here is built yet, and the backend owner can change it.

## Reading

```json
{
  "deviceId": "projector",
  "value": 80,
  "unit": "%",
  "status": "OK",
  "timestamp": "2026-11-16T19:42:10Z"
}
```

`status` is one of `OK`, `Fault`, `NoPower`, `Disconnected`.

Device ids: `speakers`, `projector`, `lights`, `airconditioning`, `motion`. Optional: `popcorn`.

## Endpoints

| Method and path | Purpose |
|---|---|
| `GET /api/devices` | The latest reading for every device |
| `GET /api/devices/{id}` | One device |
| `POST /api/commands` | Send a command to a device |
| `GET /api/events?limit=50` | The latest log entries |
| `GET /api/interrupt/settings` | No Interrupt flag, Important Numbers list, fault threshold |
| `PUT /api/interrupt/settings` | Change those settings |
| `POST /api/calls` | Simulate a call, for testing interrupt mode |

### Command

```json
{ "deviceId": "speakers", "name": "SetVolume", "value": 60 }
```

Playback commands use the device id `playback` with the names `Start`, `Pause` and `Resume`. A command that is refused (for example brightness 150) returns status 400 with `{ "error": "Brightness must be between 0 and 100" }`.

### Interrupt settings

```json
{ "noInterrupt": false, "importantNumbers": [ { "number": "5195550123", "label": "Mom" } ], "faultThreshold": 2 }
```

## Live updates (SignalR, hub at `/hub`)

The server pushes these messages to the app:

- `ReadingChanged` with a reading
- `EventRaised` with an event (`id`, `timestamp`, `deviceId`, `type`, `message`)
- `InterruptChanged` with `{ "active": true, "reason": "Call" or "Fault", "detail": "Mom" }`

## Notes

- On the Android emulator the laptop is at `10.0.2.2`. On a real phone, use the laptop's Wi-Fi address and allow the port in the firewall.
- Android blocks plain HTTP by default, so development builds need a network security config for the local address, or the ASP.NET development HTTPS certificate.

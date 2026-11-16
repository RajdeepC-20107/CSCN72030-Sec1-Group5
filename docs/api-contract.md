# Proposed API contract (SCRUM-18 and SCRUM-19)

A proposal for the data shared between the backend and the Android app. Nothing here is built yet, and the backend owner can change it. The screens that use it are described in `ui/main-screen.md`.

## Device

```json
{
  "deviceId": "overhead1",
  "name": "Overhead Spot 1",
  "type": "lights",
  "group": "Lights",
  "value": 0,
  "unit": "level",
  "status": "OK",
  "timestamp": "2026-11-16T19:42:10Z"
}
```

`status` is one of `OK`, `Fault`, `NoPower`, `Disconnected`.

`type` is one of `lights`, `speakers`, `popcorn`, `airconditioning`, `projector`, `motion`.

Example device ids: `stairway1`, `stairway2`, `overhead1` (lights), `left`, `right` (speakers), `salty`, `sweet`, `plain` (popcorn), `ac` (air conditioning), `projector`, `motion`.

Value by type: lights a dim level 0 to 100, air conditioning a target in degrees C, popcorn 0 (off) or 1 (on), projector a brightness 0 to 100, speakers a volume 0 to 100, motion the number of occupied seats. Speaker offsets (dB and ms) are in an optional `details` object, for example `{ "gainDb": 0.4, "delayMs": 200 }`.

## Endpoints

| Method and path | Purpose |
|---|---|
| `GET /api/devices` | Every device with its latest reading (Home and Device List) |
| `GET /api/devices/{id}` | One device (Device Readings) |
| `POST /api/commands` | Send a command to a device |
| `GET /api/seating` | Occupied seats and the current seating position |
| `POST /api/calibrate` | Start speaker calibration for the current position |
| `GET /api/calibrate` | Calibration progress, for example `{ "running": true, "percent": 78 }` |
| `GET /api/events?limit=50` | The latest log entries (Event Log) |
| `GET /api/interrupt/settings` | No Interrupt flag, Important Numbers list, fault threshold |
| `PUT /api/interrupt/settings` | Change those settings |
| `POST /api/calls` | Simulate a call, for testing interrupt mode |

### Command

```json
{ "deviceId": "overhead1", "name": "SetDimmer", "value": 10 }
```

Example names: `SetDimmer`, `SetPower`, `SetTemperature`, `SetVolume`, `SetBrightness`. Playback commands use the device id `playback` with the names `Start`, `Pause` and `Resume`. A command that is refused (for example brightness 150) returns status 400 with `{ "error": "Brightness must be between 0 and 100" }`.

### Interrupt settings

```json
{ "noInterrupt": false, "importantNumbers": [ { "number": "5195550123", "label": "Mom" } ], "faultThreshold": 2 }
```

## Live updates (SignalR, hub at `/hub`)

The server pushes these messages to the app:

- `ReadingChanged` with a device
- `EventRaised` with an event (`id`, `timestamp`, `deviceId`, `type`, `message`)
- `InterruptChanged` with `{ "active": true, "reason": "Call" or "Fault", "detail": "Mom" }`
- `CalibrationChanged` with `{ "running": true, "percent": 78 }`

## Kept on the phone for now

Pinned devices and the pin order are saved in the app and not in the backend. Add Device, Create Group, Rename and Remove are left for a later version.

## Notes

- On the Android emulator the laptop is at `10.0.2.2`. On a real phone, use the laptop's Wi-Fi address and allow the port in the firewall.
- Android blocks plain HTTP by default, so development builds need a network security config for the local address, or the ASP.NET development HTTPS certificate.

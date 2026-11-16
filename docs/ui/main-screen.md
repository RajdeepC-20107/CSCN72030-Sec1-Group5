# Main screen (SCRUM-19, US-2)

One Android screen where the operator sees the status of every device and can control the room.

## Sections, top to bottom

1. **Interrupt banner** (only when an interrupt is active). Shows INTERRUPT and the reason, for example "Call from Mom" or "Fault: Projector, Stereo Speakers".
2. **Controls row.** Start, Pause and Resume buttons, a maximum volume slider with its number, and a No Interrupt switch that always shows whether it is on.
3. **Device tiles.** One tile each for stereo speakers, projector, lights, air conditioning and motion sensor.
4. **Seating.** A seat map showing which seats the motion sensor reports as occupied.
5. **Event log.** The latest events, newest first.

## What each tile shows

| Field | Example |
|---|---|
| Name | Projector |
| Value and unit | 80 % |
| Status | OK |
| Last updated | 7:42:10 pm |

Tapping a controllable tile opens its controls (power, brightness, volume, balance, dim level or target temperature).

## Status states

| Status | Tile look | Text |
|---|---|---|
| OK | neutral | the value |
| Fault | red | "Fault" |
| NoPower | red | "No power" |
| Disconnected | grey | "Disconnected" |

Colour is never the only signal. Every state also has a word on the tile (UR-004).

## Rules

- A new reading appears on the screen within 1 second (NFR-003).
- Every control has a visible label, and sliders show their number (UR-005).
- Pause and Resume take no more than 2 taps (UR-003).
- If the connection to the backend drops, show a Disconnected banner and keep the last values greyed out until it reconnects.

## Data used by the screen

Each tile reads one reading: `deviceId`, `value`, `unit`, `status`, `timestamp`. See `api-contract.md`.

## Open questions

- Which Android stack and project the screen lives in.
- Whether real phone calls or only simulated calls trigger interrupt mode.

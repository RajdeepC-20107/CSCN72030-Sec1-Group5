# App screens (SCRUM-19, US-2)

The Android app is the remote control for the whole theatre. This spec follows the team's wireframe drawing. The operator sees the status of every device and controls the room from the phone.

## Screens and navigation

Home, with See All leading to Device List, which leads to Device Readings. Home also leads to Calibrate, and Profile is reached from the app bar and leads to Interrupt Mode and Event Log.

### 1. Home

- App title **AbsolutCinema**.
- **Now playing** card: the title and artist, with previous, pause or play, and next buttons. These buttons are the Start, Pause and Resume controls for the show (FR-007).
- **Speakers** card showing the current seating position. A Calibrate link opens the Calibrate screen.
- **Pinned** section with an **Edit** button. Each pinned device is a small card with its main control, for example **AC: 21 C with minus and plus**, or **Sweet: Off** for a popcorn machine.
- **See All** button that opens the Device List.

### 2. Device List

Devices grouped under headings, each row showing the device name and its current state:

| Group | Example rows |
|---|---|
| Lights | Stairway Light 1 (10, dim), Stairway Light 2 (10, dim), Overhead Spot 1 (0, off) |
| Popcorn Machines | Salty (off), Sweet (off), Plain (off) |
| Speakers | Left (+0.4 dB and +200 ms) |

A **...** menu at the top offers Add Device and Create Group. Tapping a row opens Device Readings.

### 3. Device Readings

Detail for one device, with a **...** menu offering Rename, Remove and Pin. The controls depend on the device:

- Lights: On and Off, and **Dimmer with minus and plus** (shows the number).
- Air conditioning: **Temp** with minus and plus.
- Popcorn machine: On and Off.
- Speakers: volume, balance and the dB and delay offsets.

### 4. Calibrate

Shown while the speakers adjust for the guest's seat. It has a progress ring with a percentage (for example 78%), the title **Calibrating...**, and the text "Please wait for the speakers to calibrate for your position in the room...". A Cancel button stops it.

### 5. Profile

Lists Profile, **Interrupt Mode**, **Event Log** and **Log Out**.

- **Interrupt Mode** holds the No Interrupt switch, the Important Numbers list, and the fault threshold (US-8, US-12, US-13).
- **Event Log** lists the latest events, newest first (US-6).

### Interrupt banner

On every screen, when an interrupt is active, a banner at the top shows INTERRUPT and the reason, for example "Call from Mom" or "Fault: Projector, Speakers". A No Interrupt indicator stays visible when that mode is on.

## Status states for every device row or card

| Status | Look | Text |
|---|---|---|
| OK | neutral | the value or state |
| Fault | red | "Fault" |
| NoPower | red | "No power" |
| Disconnected | grey | "Disconnected" |

Colour is never the only signal. Every state also has a word (UR-004).

## Rules

- A new reading appears on the screen within 1 second (NFR-003).
- Every control has a visible label, and sliders and steppers show their number (UR-005).
- Pause and resume take no more than 2 taps (UR-003).
- If the connection to the backend drops, show a Disconnected banner and keep the last values greyed out until it reconnects.

## Data used by the screens

Each device card or row reads one reading: `deviceId`, `value`, `unit`, `status`, `timestamp`, plus the device's `name`, `type` and `group`. See `api-contract.md`.

## Open questions

- **Projector:** the drawing has no projector, but it is one of the five controlled devices (FR-003). Add a Projector group to the Device List, with power and brightness.
- **Motion sensor:** it feeds the seating position on Home and has no row of its own.
- **Log Out:** the backend has no accounts, so decide what Log Out does or leave it out of the first version.
- **Add Device, Create Group, Rename and Remove:** the first version can load the device list from the backend and leave these for later. Pinned devices can be saved on the phone.
- Whether real phone calls or only simulated calls trigger interrupt mode.

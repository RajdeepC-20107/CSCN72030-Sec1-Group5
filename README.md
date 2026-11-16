# Absolute Cinema (CSCN72030 Section 1, Group 5)

A SCADA/HMI system for a private home theatre. Devices report their status to a central Hub, and an Android app lets the operator see and control the whole room from one screen.

## Team

Rajdeep Chowdhury, Antony Rudkovsky, Farah Lariani, Sifan Terefe

## Devices

Monitored and controlled: stereo speakers, projector, lights (dimmers), air conditioning and a seat motion sensor. A popcorn machine is a low priority extra.

Interrupt mode pauses the show and raises the lights when a call arrives from a number on an Important Numbers list, or when enough devices lose power or disconnect. The operator can switch on a No Interrupt mode to ignore calls.

## How it is built

- **Backend:** C# on .NET 10 in `SCADAHMI Project`. The DeviceListener (message bus) holds the subscribed devices, and the CommandHandler publishes commands to them.
- **Tests:** MSTest in `UnitTests`.
- **App:** an Android app that shows every device and sends commands to the backend. See `docs/ui/main-screen.md` and `docs/api-contract.md`.
- **Planned:** reading devices from looped data files, and logging every event to SQLite.

## Build and test

```
dotnet build
dotnet test
dotnet run --project "SCADAHMI Project"
```

## Working on a ticket

The work is tracked on the team's Jira board (project key SCRUM). Start every branch name and every commit message with the ticket key, for example `SCRUM-19 add device tile`, so Jira links the work to the ticket.

## Docs

- `docs/ui/main-screen.md`: what the main Android screen shows and how it behaves
- `docs/api-contract.md`: the proposed backend endpoints and data the app uses

"use strict";

// Everything is served by the API itself, so a relative URL keeps this
// client fully local - no CDN and no hard-coded host/port.
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build();

let lastVersion = 0;

const statusBadge = document.getElementById("connectionStatus");
const versionLabel = document.getElementById("version");
const sourceLabel = document.getElementById("lastSource");
const deviceList = document.getElementById("deviceList");
const logPanel = document.getElementById("log");

function log(message) {
    const time = new Date().toLocaleTimeString();
    logPanel.textContent += `[${time}] ${message}\n`;
    logPanel.scrollTop = logPanel.scrollHeight;
}

function setStatus(text, cssClass) {
    statusBadge.textContent = text;
    statusBadge.className = `badge ${cssClass}`;
}

// The typed hub declares DevicesChanged(); the API's camelCase JSON policy
// puts that method on the wire as "devicesChanged".
connection.on("devicesChanged", (snapshot) => renderSnapshot(snapshot, "SignalR push"));

connection.onreconnecting(() => setStatus("Reconnecting", "reconnecting"));
connection.onreconnected(() => {
    setStatus("Connected", "connected");
    log("SignalR connection restored.");
});
connection.onclose(() => setStatus("Disconnected", "disconnected"));

async function start() {
    try {
        await connection.start();
        setStatus("Connected", "connected");
        log("Connected to /hub - waiting for the initial snapshot from the server.");
    } catch (error) {
        setStatus("Disconnected", "disconnected");
        log(`Connection failed: ${error.message}`);
    }
}

function renderSnapshot(snapshot, source) {
    lastVersion = snapshot.version ?? 0;
    versionLabel.textContent = lastVersion;
    sourceLabel.textContent = source;

    deviceList.replaceChildren();

    if (!snapshot.devices || snapshot.devices.length === 0) {
        const empty = document.createElement("li");
        empty.className = "empty";
        empty.textContent = "No devices registered yet.";
        deviceList.appendChild(empty);
        return;
    }

    for (const device of snapshot.devices) {
        const row = document.createElement("li");

        const id = document.createElement("span");
        id.className = "device-id";
        id.textContent = device.id.substring(0, 8);

        const message = document.createElement("span");
        message.className = "device-message";
        // textContent (not innerHTML) so device data can never inject markup.
        message.textContent = device.message;

        row.append(id, message);
        deviceList.appendChild(row);
    }

    log(`Snapshot v${lastVersion} received via ${source} (${snapshot.devices.length} device(s)).`);
}

function newRequestId() {
    return crypto.randomUUID ? crypto.randomUUID() : String(Date.now());
}

async function postJson(url, body, label) {
    try {
        const response = await fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ requestId: newRequestId(), ...body })
        });

        const result = await response.json();

        if (response.ok && result.success) {
            log(`${label}: OK (requestId ${result.requestId}).`);
        } else {
            log(`${label}: FAILED (${response.status}) - ${result.error ?? "unknown error"}.`);
        }
    } catch (error) {
        log(`${label}: request error - ${error.message}`);
    }
}

async function createDevice() {
    const message = document.getElementById("createMessage").value;
    await postJson("/api/devices", { payload: { message } }, "Create device");
}

async function setMessage() {
    const message = document.getElementById("setMessage").value;
    await postJson(
        "/api/commands/set-message",
        { payload: { message } },
        "Set message"
    );
}

async function checkChanges() {
    try {
        const response = await fetch(`/api/devices?since=${lastVersion}`, {
            headers: { Accept: "application/json" }
        });

        if (response.status === 304) {
            log(`Poll since v${lastVersion}: 304 Not Modified - nothing changed.`);
            return;
        }

        if (!response.ok) {
            log(`Poll failed with status ${response.status}.`);
            return;
        }

        renderSnapshot(await response.json(), "poll");
    } catch (error) {
        log(`Poll request error - ${error.message}`);
    }
}

document.getElementById("createButton").addEventListener("click", createDevice);
document.getElementById("setButton").addEventListener("click", setMessage);
document.getElementById("checkButton").addEventListener("click", checkChanges);

start();

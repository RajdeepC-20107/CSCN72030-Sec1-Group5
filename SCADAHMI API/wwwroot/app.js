"use strict";

// Everything is served by the API itself, so a relative URL keeps this
// client fully local - no CDN and no hard-coded host/port.
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build();

let lastVersion = 0;
let currentDevices = [];

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
    currentDevices = snapshot.devices ?? [];
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
        if (device.$type === "lights") row.className = "lights";

        const info = document.createElement("div");
        info.className = "device-info";

        const name = document.createElement("span");
        name.className = "device-name";
        name.textContent = device.name ?? device.$type ?? "Device";

        if (device.$type === "lights") {
            // The list is rebuilt from every snapshot push, so the swatch is
            // recreated from the latest RGB values on each SignalR update.
            const nameLine = document.createElement("div");
            nameLine.className = "device-name-line";

            const swatch = document.createElement("span");
            swatch.className = "rgb-swatch";
            swatch.title = `RGB ${device.red}, ${device.green}, ${device.blue}`;
            swatch.style.backgroundColor =
                `rgb(${device.red}, ${device.green}, ${device.blue})`;

            nameLine.append(swatch, name);
            info.append(nameLine);
        } else {
            info.append(name);
        }

        const id = document.createElement("span");
        id.className = "device-id";
        id.textContent = device.guid.substring(0, 8);

        info.append(id);

        const controls = document.createElement("div");
        controls.className = "device-controls";

        if (device.$type === "helloDevice") {
            const input = document.createElement("input");
            input.type = "text";
            input.className = "device-input";
            input.value = device.message;
            input.setAttribute("aria-label", `Message for device ${device.guid}`);

            const setButton = document.createElement("button");
            setButton.className = "small";
            setButton.textContent = "Set";
            setButton.addEventListener("click", () =>
                sendMessages({ [device.guid]: input.value }, `Set ${device.guid.substring(0, 8)}`));

            controls.append(input, setButton);
        } else if (device.$type === "lights") {
            const props = document.createElement("span");
            props.className = "device-props";
            props.textContent =
                `RGB ${device.red},${device.green},${device.blue} · ` +
                `brightness ${device.minimumBrightness}-${device.maximumBrightness} · ` +
                `dimmed: ${device.isDimmed ? "yes" : "no"}`;
            info.append(props);

            const rgbInputs = {};
            for (const channel of ["red", "green", "blue"]) {
                const input = document.createElement("input");
                input.type = "number";
                input.min = "0";
                input.max = "255";
                input.className = "rgb-input";
                input.value = device[channel];
                input.setAttribute("aria-label", `${channel} for device ${device.guid}`);
                rgbInputs[channel] = input;
            }

            const setRgbButton = document.createElement("button");
            setRgbButton.className = "small";
            setRgbButton.textContent = "Set RGB";
            setRgbButton.addEventListener("click", () => setLightsRGB(device.guid, {
                red: rgbInputs.red.value,
                green: rgbInputs.green.value,
                blue: rgbInputs.blue.value
            }));

            controls.append(rgbInputs.red, rgbInputs.green, rgbInputs.blue, setRgbButton);
        } else {
            const label = document.createElement("span");
            label.className = "empty";
            label.textContent = `Unsupported type: ${device.$type ?? "unknown"}`;
            controls.append(label);
            log(`Skipping unsupported device type '${device.$type}' (${device.guid}).`);
        }

        // The remove payload needs this device's own discriminator, not a
        // hardcoded one, otherwise Lights cannot be removed from the UI.
        const removeButton = document.createElement("button");
        removeButton.className = "small danger";
        removeButton.textContent = "Remove";
        removeButton.title = `Remove device ${device.guid}`;
        removeButton.addEventListener("click", () => removeDevice(device));
        controls.append(removeButton);

        row.append(info, controls);
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
    await postJson("/api/devices/create", { payload: { $type: "helloDevice", message } }, "Create device");
}

// Empty means "use the device default"; out-of-range values are rejected
// locally so a typo does not silently become a different colour.
function readOptionalNumber(id, max) {
    const raw = document.getElementById(id).value.trim();
    if (raw === "") return null;
    const value = Number(raw);
    if (!Number.isInteger(value) || value < 0 || value > max) {
        log(`${id} must be a whole number between 0 and ${max}.`);
        return NaN;
    }
    return value;
}

async function createLights() {
    const payload = {
        $type: "lights",
        minimumBrightness: readOptionalNumber("createMinBrightness", 100),
        maximumBrightness: readOptionalNumber("createMaxBrightness", 100),
        red: readOptionalNumber("createRed", 255),
        green: readOptionalNumber("createGreen", 255),
        blue: readOptionalNumber("createBlue", 255)
    };
    if (Object.values(payload).some(value => Number.isNaN(value))) return;

    await postJson("/api/devices/create", { payload }, "Create Lights");
}

async function removeDevice(device) {
    await postJson(
        "/api/devices/remove",
        { payload: { $type: device.$type, guid: device.guid } },
        `Remove ${device.guid.substring(0, 8)}`
    );
}

// Only the channels present in the payload are changed. A blank input is
// omitted, which lets the server leave that channel untouched.
async function setLightsRGB(guid, channels) {
    const payload = { lights: [guid] };
    for (const [name, raw] of Object.entries(channels)) {
        const text = String(raw).trim();
        if (text === "") continue;
        const value = Number(text);
        if (!Number.isInteger(value) || value < 0 || value > 255) {
            log(`${name} must be a whole number between 0 and 255.`);
            return;
        }
        payload[name] = value;
    }

    await postJson(
        "/api/commands/set-lights-rgb",
        { payload },
        `Set RGB ${guid.substring(0, 8)}`
    );
}

async function sendMessages(messages, label) {
    await postJson(
        "/api/commands/set-message",
        { payload: { messages } },
        label
    );
}

async function setMessage() {
    const message = document.getElementById("setMessage").value;
    const messages = {};
    for (const device of currentDevices) {
        if (device.$type !== "helloDevice") continue;
        messages[device.guid] = message;
    }

    if (Object.keys(messages).length === 0) {
        log("No HelloDevices to update yet - create one first.");
        return;
    }

    await sendMessages(messages, "Set all messages");
}

async function printMessages() {
    log("Print messages: sending request...");
    await postJson("/api/commands/print-messages", {}, "Print messages");
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
document.getElementById("createLightsButton").addEventListener("click", createLights);
document.getElementById("setButton").addEventListener("click", setMessage);
document.getElementById("printButton").addEventListener("click", printMessages);
document.getElementById("checkButton").addEventListener("click", checkChanges);

start();

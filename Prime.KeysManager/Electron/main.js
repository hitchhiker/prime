const { app, BrowserWindow, ipcMain } = require('electron');

const url = require('url');
const path = require('path');
const net = require('net');

const PrimeTcpClient = require("./PrimeTcpClient");

let prime = new PrimeTcpClient();
let dataHandlerChannel = "";
let win;

function createWindow() {
    win = new BrowserWindow({ 'minWidth': 620, 'minHeight': 600 });

    win.loadURL(url.format({
        pathname: path.join(__dirname, 'index.html'),
        protocol: 'file:',
        slashes: true
    }));
}

function closeWindow() {
    alert("Windows is being closed...");
}

// TCP client.
let client = new net.Socket();

client.connect(19991, '127.0.0.1', function () {
    console.log('Connected to Prime API server.');
});

client.on('data', function (data) {
    win.webContents.send(dataHandlerChannel, data);
    console.log("Client received data. Sending to '" + dataHandlerChannel + "' channel...");
});

client.on('close', function () {
    console.log('Connection closed.');
});

// IPC Main.

ipcMain.on('prime:get-private-providers-list', (event, arg) => {
    console.log("Querying providers list...");

    client.write(JSON.stringify({
        "Type": "PrivateProvidersListMessage"
    }));
    dataHandlerChannel = "prime:private-providers-list";
});

ipcMain.on('prime:get-provider-details', (event, arg) => {
    console.log("Querying provider details...");

    client.write(JSON.stringify({
        "Type": "ProviderDetailsMessage",
        "Id": arg
    }));
    dataHandlerChannel = "prime:provider-details";
})

ipcMain.on('prime:save-provider-keys', (event, arg) => {
    console.log("Saving provider keys...");

    client.write(JSON.stringify({
        "Type": "ProviderKeysMessage",
        "Id": arg.id,
        "Key": arg.keys.Key,
        "Secret": arg.keys.Secret,
        "Extra": arg.keys.Extra
    }));
    dataHandlerChannel = "prime:provider-keys-saved";
})

ipcMain.on('prime:delete-provider-keys', (event, arg) => {
    console.log("Saving provider keys...");

    client.write(JSON.stringify({
        "Type": "DeleteProviderKeysMessage",
        "Id": arg,
    }));
    dataHandlerChannel = "prime:provider-keys-deleted";
});

ipcMain.on('prime:test-private-api', (event, arg) => {
    console.log("Testing private API...");

    client.write(JSON.stringify({
        "Type": "TestPrivateApiMessage",
        "Id": arg.id,
        "Key": arg.keys.Key,
        "Secret": arg.keys.Secret,
        "Extra": arg.keys.Extra
    }));
    dataHandlerChannel = "prime:private-api-tested";
});

app.on('close', closeWindow);
app.on('ready', createWindow);


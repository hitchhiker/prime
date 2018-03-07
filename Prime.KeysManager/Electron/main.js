const { app, BrowserWindow, ipcMain } = require('electron');

const url = require('url');
const path = require('path');

const PrimeTcpClient = require("./PrimeTcpClient");

let prime = new PrimeTcpClient();
let win;

function createWindow() {
    win = new BrowserWindow({ width: 800, height: 600 });

    win.loadURL(url.format({
        pathname: path.join(__dirname, 'index.html'),
        protocol: 'file:',
        slashes: true
    }));
}

ipcMain.on('prime:get-providers-list', (event, arg) => {
    console.log("Querying providers list...");
    //prime.getProvidersList();
});

prime.connect();

app.on('ready', createWindow);





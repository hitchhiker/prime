const { app, BrowserWindow } = require('electron')
const path = require('path')
const url = require('url')
const net = require('net');
const { ipcMain } = require('electron');

let win

function createWindow() {
  win = new BrowserWindow({ width: 800, height: 600 });

  // load the dist folder from Angular
  win.loadURL(url.format({
    pathname: path.join(__dirname, 'dist/index.html'),
    protocol: 'file:',
    slashes: true
  }))

  // Open the DevTools optionally:
  // win.webContents.openDevTools()

  win.on('closed', () => {
    win = null
  })
}

function bin2string(array){
	var result = "";
	for(var i = 0; i < array.length; ++i){
		result+= (String.fromCharCode(array[i]));
	}
	return result;
}

app.on('ready', createWindow)

var client = new net.Socket();
client.on('data', clientData);

var responseDataChannel = '';

ipcMain.on('socket:connect', (event, arg) => {
  client.connect(8082, "127.0.0.1", () => {
    console.log("Connected to Prime server.");
  });
});

ipcMain.on('socket:get-providers-list', (event, arg) => {
  responseDataChannel = 'socket:providers-list';
  client.write('{"Type":"ProvidersListMessage"}');
});

function clientData(data) {
  console.log("Sending data to '" + responseDataChannel + "' channel...");
  var stringData = bin2string(data);
  win.webContents.send(responseDataChannel, stringData);
}

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

app.on('activate', () => {
  if (win === null) {
    createWindow()
  }
})

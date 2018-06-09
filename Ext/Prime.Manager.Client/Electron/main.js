const { app, BrowserWindow, Tray, Menu } = require('electron')
const path = require('path')
const url = require('url')

let win

function createWindow() {
  win = new BrowserWindow({ width: 800, height: 600 })

  // load the dist folder from Angular
  win.loadURL(url.format({
    pathname: path.join(__dirname, 'dist/prime-manager/index.html'),
    protocol: 'file:',
    slashes: true
  }))

  // Open the DevTools optionally:
  // win.webContents.openDevTools()

  win.on("minimize", function (event) {
    event.preventDefault();
    win.hide();
  });

  win.on('close', function (event) {
    if (!app.isQuiting) {
      event.preventDefault();
      win.hide();
    }

    return false;
  });

  win.on('closed', () => {
    win = null
  })

  let imgPath = path.join(__dirname, "dist/prime-manager/assets/img/prime-17.ico");
  let trayIcon = new Tray(imgPath);

  const trayMenuTemplate = [
    {
      label: 'Prime.Manager',
      enabled: false
    },
    {
      label: 'Show',
      click: function () {
        win.show();
      }
    },
    {
      label: 'Quit',
      click: function () {
        app.isQuiting = true;
        app.quit();
      }
    }
  ];

  let trayMenu = Menu.buildFromTemplate(trayMenuTemplate);
  trayIcon.setContextMenu(trayMenu);

  console.log(__dirname);
}

app.on('ready', createWindow)

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

app.on('activate', () => {
  if (win === null) {
    createWindow();
  }
})
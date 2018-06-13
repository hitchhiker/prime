const { app, BrowserWindow, Tray, Menu } = require('electron');
const path = require('path');
const url = require('url');
const fs = require('fs');

const platformWin32 = "win32";
const platformDarwin = "darwin";

let win = null;
let isDevFolder = checkIfDevFolder();

run();

// --- Functions --- //

function run() {
  runDarwin(() => {
    app.dock.hide();
  });
  
  app.on('ready', appReady);

  /*app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
      app.quit();
    }
  });

  app.on('activate', () => {
    if (win === null) {
      createWindow();
    }
  });*/
}

function appReady() {
  createTray();

  createWindow();
}

function createTray() {
  let iconFilename = "";
  switch (process.platform) {
    case platformDarwin:
      iconFilename = "prime-tray-unix.png";
      break;
    case platformWin32:
      iconFilename = "prime-tray-win.ico";
      break;
  }

  let iconPath;

  if (isDevFolder) {
    iconPath = path.join('assets/img', iconFilename);
  } else {
    iconPath = path.join('dist/prime-manager/assets/img', iconFilename);
  }

  let iconFullPath = path.join(__dirname, iconPath);

  let trayIcon = new Tray(iconFullPath);
  trayIcon.setToolTip("Prime.Manager");

  let trayMenuTemplate = createTrayMenuTemplate();
  let trayMenu = Menu.buildFromTemplate(trayMenuTemplate);

  trayIcon.setContextMenu(trayMenu);
}

function createTrayMenuTemplate() {
  return [
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
}

function createWindow() {
  win = new BrowserWindow({ width: 800, height: 600, maximizable: false })

  let pathIndex = 'dist/prime-manager/index.html';

  if (isDevFolder) {
    pathIndex = 'index.html';
  }

  // load the dist folder from Angular
  win.loadURL(url.format({
    pathname: path.join(__dirname, pathIndex),
    protocol: 'file:',
    slashes: true
  }))

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
    win = null;
  });
}

function runWin(func) {
  if(process.platform === platformWin32) {
    func();
  }
}

function runDarwin(func) {
  if(process.platform === platformDarwin) {
    func();
  }
}

function checkIfDevFolder() {
  return fs.existsSync(path.join(__dirname, "index.html"));
}


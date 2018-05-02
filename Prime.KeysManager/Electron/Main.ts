import { BrowserWindow } from 'electron';
import { Logger } from './src/utils/Logger';
import { TcpClient } from './src/transport/TcpClient';
import { KeysManager } from './src/core/KeysManager';

export default class Main {
    static mainWindow: Electron.BrowserWindow;
    static application: Electron.App;
    static BrowserWindow: typeof BrowserWindow;

    private static onWindowAllClosed() {
        if (process.platform !== 'darwin')
            Main.application.quit();
    }

    private static onClose() {
        Main.mainWindow = null;
    }

    private static onReady() {
        Main.mainWindow = new Main.BrowserWindow(
            {
                width: 800,
                height: 600,
                minHeight: 620,
                minWidth: 600
            }
        )
        Main.mainWindow.loadURL('file://' + __dirname + '/index.html');
        Main.mainWindow.on('closed', Main.onClose);

        // Start KeysManager.
        let keysManager = new KeysManager();
        keysManager.run();
    }

    static main(app: Electron.App, browserWindow: typeof BrowserWindow) {
        Main.BrowserWindow = browserWindow;

        Main.application = app;
        Main.application.on('window-all-closed', Main.onWindowAllClosed);
        Main.application.on('ready', Main.onReady);
    }
}
import { ipcMain, ipcRenderer, WebContents } from "electron";
import { IIpcMessage } from "./msgs/IIpsMessage";
import { HelloMessage } from "./msgs/HelloMessage";

export class IpcManager {
    private static ipcManager: IpcManager = null;
    static i(): IpcManager {
        if (IpcManager.ipcManager === null) {
            IpcManager.ipcManager = new IpcManager();
        }
        return IpcManager.ipcManager;
    }

    private constructor() {

    }

    helloMessage: IIpcMessage = new HelloMessage();
}


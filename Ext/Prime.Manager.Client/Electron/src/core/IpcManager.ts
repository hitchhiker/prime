import { ipcMain, ipcRenderer, WebContents } from "electron";
import { IIpcMessage } from "./msgs/IIpcMessage";
import { HelloMessage, GetPrivateProvidersListMessage, GetProviderDetailsMessage, SaveProviderKeysMessage, DeleteProviderKeysMessage, TestPrivateApiMessage } from "./msgs/Messages";

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
    getPrivateProvidersListMessage: IIpcMessage = new GetPrivateProvidersListMessage();
    getProviderDetailsMessage: IIpcMessage = new GetProviderDetailsMessage();
    saveProviderKeysMessage: IIpcMessage = new SaveProviderKeysMessage(); 
    deleteProviderKeysMessage: IIpcMessage = new DeleteProviderKeysMessage();
    testPrivateApiMessage: IIpcMessage = new TestPrivateApiMessage;
}

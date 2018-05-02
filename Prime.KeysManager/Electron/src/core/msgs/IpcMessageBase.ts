import { IIpcMessage } from "./IIpsMessage";
import { ipcRenderer, ipcMain, WebContents } from "electron";

// Base class for all messages.
export abstract class IpcMessageBase implements IIpcMessage {
    private requestChannel: string = null;
    private responseChannel: string = null;

    protected constructor() {
        let channelBase = this.channelBaseId();
        this.requestChannel = channelBase + "-req";
        this.responseChannel = channelBase + "-resp";
    }

    protected abstract channelBaseId(): string;

    call(dataString: any, callback: (event: any, data: any) => void) {
        ipcRenderer.send(this.requestChannel, dataString);
        ipcRenderer.on(this.responseChannel, callback);
    }

    handle(callback: (event: any, data: any) => string) {
        ipcMain.on(this.requestChannel, (event, data) => {
            let sender: WebContents = event.sender;
            let responseData = callback(sender, data);

            // Sends data back only if not NULL returned.
            if(responseData !== null)
                sender.send(this.responseChannel, responseData);
        });
    }
}

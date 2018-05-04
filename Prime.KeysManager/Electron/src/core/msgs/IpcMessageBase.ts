import { IIpcMessage } from "./IIpcMessage";
import { ipcRenderer, ipcMain, WebContents } from "electron";
import {IIpcHandlerContext} from "../models/IIpcHandlerContext";

// Base class for all messages.
export abstract class IpcMessageBase implements IIpcMessage {
    private requestChannel: string = null;
    private responseChannel: string = null;

    privat 

    private lastSender : WebContents = null;

    public constructor() {
        let channelBase = this.channelBaseId();
        this.requestChannel = channelBase + "-req";
        this.responseChannel = channelBase + "-resp";
    }

    protected abstract channelBaseId(): string;

    call(dataString: any, callback: (event: any, data: any) => void) {
        ipcRenderer.send(this.requestChannel, dataString);
        ipcRenderer.removeListener(this.responseChannel, callback);
        ipcRenderer.on(this.responseChannel, callback);
    }

    callBackLast(data: any) {
        this.lastSender.send(this.responseChannel, data);
    }

    handle(callback: (context: IIpcHandlerContext) => string) {
        ipcMain.on(this.requestChannel, (event, data) => {
            let sender: WebContents = event.sender;

            this.lastSender = sender;

            let innerContext : IIpcHandlerContext = {
                sender: sender,
                data: data,
                requestChannel: this.requestChannel,
                responseChannel: this.responseChannel,
                ipcMessageCaller: this
            };

            let responseData = callback(innerContext);

            // Sends data back only if not NULL returned.
            if (responseData !== null)
                sender.send(this.responseChannel, responseData);
        });
    }

    callResponse
}

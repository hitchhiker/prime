import { WebContents } from "electron";
import { IIpcMessage } from "../msgs/IIpcMessage";

export interface IIpcHandlerContext {
    sender: WebContents;
    data: any;
    requestChannel: string;
    responseChannel: string;
    ipcMessageCaller: IIpcMessage;
}

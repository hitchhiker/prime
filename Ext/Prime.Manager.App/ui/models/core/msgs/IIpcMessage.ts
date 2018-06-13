import { WebContents } from "electron";
import { IIpcHandlerContext } from "../models/IIpcHandlerContext";

export interface IIpcMessage {
    call(dataString: any, callback: (event, data) => void);
    callBackLast(data: any);
    handle(callback: (context: IIpcHandlerContext) => string);
}

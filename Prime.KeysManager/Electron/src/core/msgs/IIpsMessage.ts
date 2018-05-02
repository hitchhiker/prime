import { WebContents } from "electron";

export interface IIpcMessage {
    call(dataString: any, callback: (event, data) => void);
    handle(callback: (sender: WebContents, data) => string);
}

import { IpcMessageBase } from "./IpcMessageBase";

// Example of IpcMessageBase usage.
export class HelloMessage extends IpcMessageBase {
    constructor() {
        super();
    }

    protected channelBaseId(): string {
        return "hello-msg";
    }
}
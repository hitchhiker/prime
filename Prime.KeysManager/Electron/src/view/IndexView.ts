import { Logger } from "../utils/Logger";
import { IpcManager } from "../core/IpcManager";

export class IndexView {
    private ipcManager: IpcManager = IpcManager.i();

    constructor() {
        Logger.log("view call");
    }

    run() {
        Logger.log("Index run");
        this.ipcManager.helloMessage.call(JSON.stringify({ age: 22, name: "Alex" }), (event, data) => {
            console.log(data);
        });
    }
}


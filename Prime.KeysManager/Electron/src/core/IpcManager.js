"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var HelloMessage_1 = require("./msgs/HelloMessage");
var IpcManager = /** @class */ (function () {
    function IpcManager() {
        this.helloMessage = new HelloMessage_1.HelloMessage();
    }
    IpcManager.i = function () {
        if (IpcManager.ipcManager === null) {
            IpcManager.ipcManager = new IpcManager();
        }
        return IpcManager.ipcManager;
    };
    IpcManager.ipcManager = null;
    return IpcManager;
}());
exports.IpcManager = IpcManager;
//# sourceMappingURL=IpcManager.js.map
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Messages_1 = require("./msgs/Messages");
var IpcManager = /** @class */ (function () {
    function IpcManager() {
        this.helloMessage = new Messages_1.HelloMessage();
        this.getPrivateProvidersListMessage = new Messages_1.GetPrivateProvidersListMessage();
        this.getProviderDetailsMessage = new Messages_1.GetProviderDetailsMessage();
        this.saveProviderKeysMessage = new Messages_1.SaveProviderKeysMessage();
        this.deleteProviderKeysMessage = new Messages_1.DeleteProviderKeysMessage();
        this.testPrivateApiMessage = new Messages_1.TestPrivateApiMessage;
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
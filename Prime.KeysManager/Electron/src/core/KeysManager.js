"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Logger_1 = require("../utils/Logger");
var electron_1 = require("electron");
var PrimeTcpClient_1 = require("./PrimeTcpClient");
var KeysManager = /** @class */ (function () {
    function KeysManager() {
        this.primeTcpClient = new PrimeTcpClient_1.PrimeTcpClient();
    }
    KeysManager.prototype.run = function () {
        this.primeTcpClient.connect();
        this.primeTcpClient.registerIpc();
        this.registerIpc();
    };
    KeysManager.prototype.registerIpc = function () {
        // IpcManager.i().getPrivateProvidersListMessage.handle((context: IIpcHandlerContext) => {
        //     Logger.log("Querying providers list...");
        var _this = this;
        //     this.tcpClient.write(JSON.stringify({
        //         "Type": "PrivateProvidersListMessage"
        //     }));
        //     return "";
        // });
        // IpcMain.on('hello-main', (e, data) => {
        //     let sender: WebContents = e.sender;
        //     console.log(data);
        //     sender.send('hello-renderer', JSON.stringify({main:'Me'}));
        // });
        electron_1.ipcMain.on('prime:generate-client-guid', function (event, arg) {
            Logger_1.Logger.log("Calling server to generate GUID...");
            _this.tcpClient.write(JSON.stringify({
                "Type": "GenerateGuidMessage",
            }));
            _this.dataHandlerChannel = "prime:client-guid-generated";
        });
        electron_1.ipcMain.on('prime:get-private-providers-list', function (event, arg) {
            Logger_1.Logger.log("Querying providers list...");
            _this.tcpClient.write(JSON.stringify({
                "Type": "PrivateProvidersListMessage"
            }));
            _this.dataHandlerChannel = "prime:private-providers-list";
        });
        electron_1.ipcMain.on('prime:get-provider-details', function (event, arg) {
            Logger_1.Logger.log("Querying provider details...");
            _this.tcpClient.write(JSON.stringify({
                "Type": "ProviderDetailsMessage",
                "Id": arg
            }));
            _this.dataHandlerChannel = "prime:provider-details";
        });
        electron_1.ipcMain.on('prime:save-provider-keys', function (event, arg) {
            Logger_1.Logger.log("Saving provider keys...");
            _this.tcpClient.write(JSON.stringify({
                "Type": "ProviderKeysMessage",
                "Id": arg.id,
                "Key": arg.keys.Key,
                "Secret": arg.keys.Secret,
                "Extra": arg.keys.Extra
            }));
            _this.dataHandlerChannel = "prime:provider-keys-saved";
        });
        electron_1.ipcMain.on('prime:delete-provider-keys', function (event, arg) {
            Logger_1.Logger.log("Saving provider keys...");
            _this.tcpClient.write(JSON.stringify({
                "Type": "DeleteProviderKeysMessage",
                "Id": arg,
            }));
            _this.dataHandlerChannel = "prime:provider-keys-deleted";
        });
        electron_1.ipcMain.on('prime:test-private-api', function (event, arg) {
            Logger_1.Logger.log("Testing private API...");
            _this.tcpClient.write(JSON.stringify({
                "Type": "TestPrivateApiMessage",
                "Id": arg.id,
                "Key": arg.keys.Key,
                "Secret": arg.keys.Secret,
                "Extra": arg.keys.Extra
            }));
            _this.dataHandlerChannel = "prime:private-api-tested";
        });
    };
    return KeysManager;
}());
exports.KeysManager = KeysManager;
//# sourceMappingURL=KeysManager.js.map
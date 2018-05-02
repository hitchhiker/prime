"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Logger_1 = require("../utils/Logger");
var TcpClient_1 = require("../transport/TcpClient");
var Main_1 = require("../../Main");
var electron_1 = require("electron");
var IpcManager_1 = require("./IpcManager");
var KeysManager = /** @class */ (function () {
    function KeysManager() {
    }
    KeysManager.prototype.run = function () {
        this.startClient();
        this.registerIpc();
    };
    KeysManager.prototype.startClient = function () {
        var _this = this;
        Logger_1.Logger.log("Starting TCP client...");
        this.tcpClient = new TcpClient_1.TcpClient();
        this.tcpClient.onClientConnected = function () {
            Logger_1.Logger.log("Connected to Prime API server.");
        };
        this.tcpClient.onDataReceived = function (data) {
            Main_1.default.mainWindow.webContents.send(_this.dataHandlerChannel, data);
            Logger_1.Logger.log("Client received data. Sending to '" + _this.dataHandlerChannel + "' channel...");
        };
        this.tcpClient.onConnectionClosed = function () {
            Logger_1.Logger.log("Connection closed.");
        };
        this.tcpClient.connect(19991, '127.0.0.1');
    };
    KeysManager.prototype.registerIpc = function () {
        var _this = this;
        IpcManager_1.IpcManager.i().helloMessage.handle(function (sender, data) {
            console.log(data);
            return JSON.stringify({ main: 'Me' });
        });
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
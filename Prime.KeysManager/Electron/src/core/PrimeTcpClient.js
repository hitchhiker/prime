"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var IpcManager_1 = require("./IpcManager");
var Logger_1 = require("../utils/Logger");
var TcpClient_1 = require("../transport/TcpClient");
// NodeJS client. Should be used in Main process of Electron.
var PrimeTcpClient = /** @class */ (function () {
    function PrimeTcpClient() {
        this.tcpClientDataReceivedCallbackMessage = null;
    }
    PrimeTcpClient.prototype.connect = function () {
        var _this = this;
        Logger_1.Logger.log("Starting TCP client...");
        this.tcpClient = new TcpClient_1.TcpClient();
        this.tcpClient.onClientConnected = function () {
            Logger_1.Logger.log("Connected to Prime API server.");
        };
        this.tcpClient.onDataReceived = function (data) {
            if (_this.tcpClientDataReceivedCallbackMessage !== null) {
                _this.tcpClientDataReceivedCallbackMessage.callBackLast(data);
                Logger_1.Logger.log("Client received data. Sending to last channel...");
            }
            else {
                Logger_1.Logger.log("tcpClientDataReceivedCallbackMessage is null.");
            }
        };
        this.tcpClient.onConnectionClosed = function () {
            Logger_1.Logger.log("Connection closed.");
        };
        this.tcpClient.connect(19991, '127.0.0.1');
    };
    PrimeTcpClient.prototype.registerIpc = function () {
        var _this = this;
        IpcManager_1.IpcManager.i().getPrivateProvidersListMessage.handle(function (context) {
            Logger_1.Logger.log("Querying providers list...");
            _this.tcpClient.write(JSON.stringify({
                "Type": "PrivateProvidersListMessage"
            }));
            console.log(context);
            _this.tcpClientDataReceivedCallbackMessage = context.ipcMessageCaller;
            return null;
        });
    };
    PrimeTcpClient.prototype.getPrivateProvidersList = function (callback) {
        IpcManager_1.IpcManager.i().getPrivateProvidersListMessage.call(null, callback);
    };
    PrimeTcpClient.prototype.getProviderDetails = function () {
    };
    return PrimeTcpClient;
}());
exports.PrimeTcpClient = PrimeTcpClient;
//# sourceMappingURL=PrimeTcpClient.js.map
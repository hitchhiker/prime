"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var net_1 = require("net");
var TcpClient = /** @class */ (function () {
    function TcpClient() {
        this.socketClient = new net_1.Socket();
    }
    TcpClient.prototype.connect = function (port, host) {
        this.socketClient.on('data', this.onDataReceived);
        this.socketClient.on('close', this.onConnectionClosed);
        this.socketClient.connect(port, host, this.onClientConnected);
    };
    TcpClient.prototype.write = function (data) {
        this.socketClient.write(data);
    };
    return TcpClient;
}());
exports.TcpClient = TcpClient;
//# sourceMappingURL=TcpClient.js.map
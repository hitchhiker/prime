"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var electron_1 = require("electron");
// Base class for all messages.
var IpcMessageBase = /** @class */ (function () {
    function IpcMessageBase() {
        this.requestChannel = null;
        this.responseChannel = null;
        this.lastSender = null;
        var channelBase = this.channelBaseId();
        this.requestChannel = channelBase + "-req";
        this.responseChannel = channelBase + "-resp";
    }
    IpcMessageBase.prototype.call = function (dataString, callback) {
        electron_1.ipcRenderer.send(this.requestChannel, dataString);
        electron_1.ipcRenderer.removeListener(this.responseChannel, callback);
        electron_1.ipcRenderer.on(this.responseChannel, callback);
    };
    IpcMessageBase.prototype.callBackLast = function (data) {
        this.lastSender.send(this.responseChannel, data);
    };
    IpcMessageBase.prototype.handle = function (callback) {
        var _this = this;
        electron_1.ipcMain.on(this.requestChannel, function (event, data) {
            var sender = event.sender;
            _this.lastSender = sender;
            var innerContext = {
                sender: sender,
                data: data,
                requestChannel: _this.requestChannel,
                responseChannel: _this.responseChannel,
                ipcMessageCaller: _this
            };
            var responseData = callback(innerContext);
            // Sends data back only if not NULL returned.
            if (responseData !== null)
                sender.send(_this.responseChannel, responseData);
        });
    };
    return IpcMessageBase;
}());
exports.IpcMessageBase = IpcMessageBase;
//# sourceMappingURL=IpcMessageBase.js.map
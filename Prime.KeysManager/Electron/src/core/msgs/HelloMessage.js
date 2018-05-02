"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var IpcMessageBase_1 = require("./IpcMessageBase");
// Example of IpcMessageBase usage.
var HelloMessage = /** @class */ (function (_super) {
    __extends(HelloMessage, _super);
    function HelloMessage() {
        return _super.call(this) || this;
    }
    HelloMessage.prototype.channelBaseId = function () {
        return "hello-msg";
    };
    return HelloMessage;
}(IpcMessageBase_1.IpcMessageBase));
exports.HelloMessage = HelloMessage;
//# sourceMappingURL=HelloMessage.js.map
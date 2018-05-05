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
        return _super !== null && _super.apply(this, arguments) || this;
    }
    HelloMessage.prototype.channelBaseId = function () {
        return HelloMessage.name;
    };
    return HelloMessage;
}(IpcMessageBase_1.IpcMessageBase));
exports.HelloMessage = HelloMessage;
var GetPrivateProvidersListMessage = /** @class */ (function (_super) {
    __extends(GetPrivateProvidersListMessage, _super);
    function GetPrivateProvidersListMessage() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    GetPrivateProvidersListMessage.prototype.channelBaseId = function () {
        return GetPrivateProvidersListMessage.name;
    };
    return GetPrivateProvidersListMessage;
}(IpcMessageBase_1.IpcMessageBase));
exports.GetPrivateProvidersListMessage = GetPrivateProvidersListMessage;
var GetProviderDetailsMessage = /** @class */ (function (_super) {
    __extends(GetProviderDetailsMessage, _super);
    function GetProviderDetailsMessage() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    GetProviderDetailsMessage.prototype.channelBaseId = function () {
        return GetProviderDetailsMessage.name;
    };
    return GetProviderDetailsMessage;
}(IpcMessageBase_1.IpcMessageBase));
exports.GetProviderDetailsMessage = GetProviderDetailsMessage;
var SaveProviderKeysMessage = /** @class */ (function (_super) {
    __extends(SaveProviderKeysMessage, _super);
    function SaveProviderKeysMessage() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    SaveProviderKeysMessage.prototype.channelBaseId = function () {
        return SaveProviderKeysMessage.name;
    };
    return SaveProviderKeysMessage;
}(IpcMessageBase_1.IpcMessageBase));
exports.SaveProviderKeysMessage = SaveProviderKeysMessage;
var DeleteProviderKeysMessage = /** @class */ (function (_super) {
    __extends(DeleteProviderKeysMessage, _super);
    function DeleteProviderKeysMessage() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    DeleteProviderKeysMessage.prototype.channelBaseId = function () {
        return DeleteProviderKeysMessage.name;
    };
    return DeleteProviderKeysMessage;
}(IpcMessageBase_1.IpcMessageBase));
exports.DeleteProviderKeysMessage = DeleteProviderKeysMessage;
var TestPrivateApiMessage = /** @class */ (function (_super) {
    __extends(TestPrivateApiMessage, _super);
    function TestPrivateApiMessage() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    TestPrivateApiMessage.prototype.channelBaseId = function () {
        return TestPrivateApiMessage.name;
    };
    return TestPrivateApiMessage;
}(IpcMessageBase_1.IpcMessageBase));
exports.TestPrivateApiMessage = TestPrivateApiMessage;
//# sourceMappingURL=Messages.js.map
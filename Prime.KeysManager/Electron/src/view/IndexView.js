"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Logger_1 = require("../utils/Logger");
var IpcManager_1 = require("../core/IpcManager");
var IndexView = /** @class */ (function () {
    function IndexView() {
        this.ipcManager = IpcManager_1.IpcManager.i();
        Logger_1.Logger.log("view call");
    }
    IndexView.prototype.run = function () {
        Logger_1.Logger.log("Index run");
        this.ipcManager.helloMessage.call(JSON.stringify({ age: 22, name: "Alex" }), function (event, data) {
            console.log(data);
        });
    };
    return IndexView;
}());
exports.IndexView = IndexView;
//# sourceMappingURL=IndexView.js.map
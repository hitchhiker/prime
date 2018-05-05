"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ActionThrottler = /** @class */ (function () {
    function ActionThrottler() {
        this.timer = null;
    }
    ActionThrottler.prototype.throttle = function (period, action) {
        clearTimeout(this.timer);
        this.timer = setTimeout(function () {
            action();
        }, period);
    };
    return ActionThrottler;
}());
exports.ActionThrottler = ActionThrottler;
//# sourceMappingURL=ActionThrottler.js.map
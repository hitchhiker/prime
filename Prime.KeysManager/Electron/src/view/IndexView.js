"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Logger_1 = require("../utils/Logger");
var IpcManager_1 = require("../core/IpcManager");
var $ = require("jquery");
var IndexView = /** @class */ (function () {
    function IndexView() {
        this.ipcManager = IpcManager_1.IpcManager.i();
    }
    IndexView.prototype.run = function () {
        Logger_1.Logger.log("Index run");
        this.ipcManager.helloMessage.call(JSON.stringify({ age: 22, name: "Alex" }), function (event, data) {
            console.log(data);
        });
        this.getPrivateProviders();
    };
    IndexView.prototype.getPrivateProviders = function () {
        var _this = this;
        IpcManager_1.IpcManager.i().getPrivateProvidersListMessage.call(null, function (event, data) {
            _this.providersList = JSON.parse(data);
            _this.displayProviders(_this.providersList);
        });
    };
    IndexView.prototype.displayProviders = function (providersList) {
        var html = "";
        for (var i = 0; i < providersList.length; i++) {
            var provider = providersList[i];
            html +=
                "<div class=\"row\">\n                    <div class=\"col s12\">\n                    <div class=\"card blue-grey darken-1 \">\n                        <div class=\"card-content white-text\">\n                            " + (provider.HasKeys === true ? '<i class="right material-icons has-api-keys-icon">vpn_key</i>' : '') + "\n                            <span class=\"card-title\">" + provider.Name + "</span>\n                            <p class=\"italic grey-text text-lighten-3 provider-id\">" + provider.Id + "</p>\n                        </div>\n                        <div class=\"card-action\">\n                            <a class=\"waves-effect waves-light btn modal-trigger light-green darken-2\" href=\"#editProviderModal\">Manage</a>\n                        </div>\n                    </div>\n                    </div>\n                </div>";
        }
        $("#providers-list").html(html);
    };
    return IndexView;
}());
exports.IndexView = IndexView;
//# sourceMappingURL=IndexView.js.map
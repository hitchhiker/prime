"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var IpcManager_1 = require("../core/IpcManager");
var $ = require("jquery");
var ActionThrottler_1 = require("../core/ActionThrottler");
var IndexView = /** @class */ (function () {
    function IndexView() {
        this.ipcManager = IpcManager_1.IpcManager.i();
        this.inputSearchProvider = $('#searchProvider');
        this.inputPublicKey = $('#publicKey');
        this.inputPrivateKey = $('#privateKey');
        this.inputExtra = $('#extra');
        this.inputProviderId = $('#providerId');
        this.editProviderModal = $('#editProviderModal');
        this.actionThrottler = new ActionThrottler_1.ActionThrottler();
    }
    IndexView.prototype.run = function () {
        var _this = this;
        this.inputSearchProvider.on('input', function () {
            _this.actionThrottler.throttle(300, function () {
                var searchValue = _this.inputSearchProvider.val();
                var filteredProvidersList = _this.providersList.filter(function (p) { return p.Name.toLowerCase().indexOf(searchValue.toLowerCase()) !== -1; });
                _this.displayProviders(filteredProvidersList);
            });
        });
        this.editProviderModal = $(document).find('#editProviderModal');
        console.log(this.inputSearchProvider);
        $(document).on("click", ".modal-trigger", function (e) { _this.showProviderDetails(e); });
        $(document).on("click", "#saveAndClose", function (e) { _this.saveProviderDetails(e); });
        $(document).on("click", "#removeKeys", function (e) { _this.removeProviderKeys(e); });
        $(document).on("click", "#testPrivateApi", function (e) { _this.testPrivateApi(e); });
        this.getPrivateProviders();
        this.initModal();
    };
    IndexView.prototype.testPrivateApi = function (e) {
        var id = this.inputProviderId.val();
        var keys = {
            Key: this.inputPublicKey.val(),
            Secret: this.inputPrivateKey.val(),
            Extra: this.inputExtra.val()
        };
        this.ipcManager.testPrivateApiMessage.call({ id: id, keys: keys }, this.testPrivateApiCall);
    };
    IndexView.prototype.testPrivateApiCall = function (event, data) {
        var response = JSON.parse(data);
        var msg = 'Private API test SUCCEEDED';
        if (response.Success === false)
            msg = 'Private API test FAILED';
        Materialize.toast(msg, 4000);
    };
    IndexView.prototype.removeProviderKeys = function (e) {
        var id = this.inputProviderId.val();
        this.ipcManager.deleteProviderKeysMessage.call(id, this.deleteProviderKeysMessageCall);
    };
    IndexView.prototype.deleteProviderKeysMessageCall = function (event, data) {
        var response = JSON.parse(data);
        var msg = 'Keys successfully deleted';
        if (response.Success === false)
            msg = 'Error during keys deleting';
        Materialize.toast(msg, 4000);
    };
    IndexView.prototype.saveProviderDetails = function (e) {
        var id = this.inputProviderId.val();
        var keys = {
            Key: this.inputPublicKey.val(),
            Secret: this.inputPrivateKey.val(),
            Extra: this.inputExtra.val()
        };
        this.ipcManager.saveProviderKeysMessage.call({ id: id, keys: keys }, function (event, data) {
            var response = JSON.parse(data);
            var msg = 'Keys successfully saved';
            if (response.Success === false)
                msg = 'Error during keys saving';
            Materialize.toast(msg, 4000);
        });
    };
    IndexView.prototype.showProviderDetails = function (e) {
        var _this = this;
        var providerCard = $(e.currentTarget).parents(".card");
        var providerId = providerCard.find(".provider-id").text();
        var providerName = providerCard.find(".card-title").text();
        //console.log($('#editProviderModal'));
        this.editProviderModal.find('.title').text("Edit " + providerName + " keys");
        this.ipcManager.getProviderDetailsMessage.call(providerId, function (event, data) {
            var provider = JSON.parse(data);
            _this.inputPrivateKey.val(provider.Secret).focus();
            _this.inputExtra.val(provider.Extra).focus();
            _this.inputProviderId.val(provider.Id);
            _this.inputPublicKey.val(provider.Key).focus();
        });
    };
    IndexView.prototype.getPrivateProviders = function () {
        var _this = this;
        this.ipcManager.getPrivateProvidersListMessage.call(null, function (event, data) {
            _this.providersList = JSON.parse(data);
            _this.displayProviders(_this.providersList);
        });
    };
    IndexView.prototype.initModal = function () {
        $('.modal').modal({
            dismissible: true,
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
const { ipcRenderer } = require("electron");

var PrimeTcpClient = function () {
    var self = this;

    var internalData = null;
    this.client = null;
    this.clientGuid = null;

    this.connect = function (callback) {
        ipcRenderer.send('prime:generate-client-guid');

        registerCallback('prime:client-guid-generated', callback);
    }

    let registeredCallbacks = [];
    function registerCallback(channel, callback) {
        if (!registeredCallbacks.includes(channel)) {
            ipcRenderer.on(channel, callback);
            registeredCallbacks.push(channel);
        }
    }

    this.getPrivateProvidersList = function (callback) {
        ipcRenderer.send('prime:get-private-providers-list');

        registerCallback('prime:private-providers-list', callback);
    }

    this.getProviderDetails = function (id, callback) {
        ipcRenderer.send('prime:get-provider-details', id);

        registerCallback('prime:provider-details', callback);
    }

    this.saveProviderKeys = function (id, keys, callback) {
        ipcRenderer.send('prime:save-provider-keys', { id: id, keys: keys });

        registerCallback('prime:provider-keys-saved', callback);
    }

    this.deleteProviderKeys = function (id, callback) {
        ipcRenderer.send('prime:delete-provider-keys', id);

        registerCallback('prime:provider-keys-deleted', callback);
    }

    this.testPrivateApi = function(id, keys, callback) {
        ipcRenderer.send('prime:test-private-api', { id: id, keys: keys });

        registerCallback('prime:private-api-tested', callback);
    }
}

module.exports = PrimeTcpClient;

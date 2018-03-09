const {ipcRenderer} = require("electron");

var PrimeTcpClient = function () {
    var self = this;

    var internalData = null;
    this.client = null;
    this.connect = function () {

    }

    this.getPrivateProvidersList = function (callback) {
        ipcRenderer.send('prime:get-private-providers-list');

        ipcRenderer.on('prime:private-providers-list', callback);
    }

    this.getProviderDetails = function(id, callback) {
        ipcRenderer.send('prime:get-provider-details', id);

        ipcRenderer.on('prime:provider-details', callback);
    }

    this.saveProviderKeys = function(id, keys, callback) {
        ipcRenderer.send('prime:save-provider-keys', { id: id, keys: keys });

        ipcRenderer.on('prime:provider-keys-saved', callback);
    }
}

module.exports = PrimeTcpClient;
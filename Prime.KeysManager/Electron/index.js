var { ipcRenderer } = require("electron");
const net = require('net');

var IndexView = function () {
    let self = this;
    self.run = function () {
        var client = new net.Socket();

        client.connect(8082, '127.0.0.1', function () {
            console.log('Connected to Prime API server');
        });

        client.on('data', function (data) {
            var providers = JSON.parse(data);
            var html = ""; 

            for (var i = 0; i < providers.length; i++) {
                var provider = providers[i];
                html += '<div class="list-item">' +
                            '<div class="title">' + provider.Name + '</div>' +
                            '<div class="id">' + provider.Id + '</div>' +
                        '</div>';
            }

            $("#providers-list").html(html);
        });

        client.on('close', function () {
            console.log('Connection closed');
        });

        client.write('{"Type":"ProvidersListMessage"}');
    }
}

var indexView = new IndexView();
indexView.run();
const { ipcRenderer } = require("electron");
const PrimeTcpClient = require("./PrimeTcpClient");
const ActionThrottler = require("./ActionThrottler");

const $ = require("jquery");

const net = require('net');

var IndexView = function () {
    let self = this;
    let primeTcpClient = new PrimeTcpClient();
    let throttler = new ActionThrottler();
    let providersList = [];

    let inputSearchProvider = $('#searchProvider');
    let inputPublicKey = $('#publicKey');
    let inputPrivateKey = $('#privateKey');
    let inputExtra = $('#extra');
    let inputProviderId = $('#providerId');

    let editProviderModal = $('#editProviderModal');

    self.run = function () {
        inputSearchProvider.on('input', function () {
            throttler.throttle(300, function () {
                let searchValue = inputSearchProvider.val();
                let filteredProvidersList = providersList.filter(p => p.Name.toLowerCase().indexOf(searchValue.toLowerCase()) !== -1);
                displayProviders(filteredProvidersList);
            });
        });

        $(document).on("click", ".modal-trigger", showProviderDetails);
        $(document).on("click", "#saveAndClose", saveProviderDetails);
        $(document).on("click", "#removeKeys", removeProviderKeys);
        $(document).on("click", "#testPrivateApi", testPrivateApi);

        getPrivateProviders();
        initModal();

        // primeTcpClient.connect((event, data) => {
        //     let response = JSON.parse(data);

        //     primeTcpClient.clientGuid = response;
        //     console.log(primeTcpClient.clientGuid);
        // });
    }

    function testPrivateApi(e) {
        let id = inputProviderId.val();

        let keys = {
            Key: inputPublicKey.val(),
            Secret: inputPrivateKey.val(),
            Extra: inputExtra.val()
        };

        primeTcpClient.testPrivateApi(id, keys, (event, data) => {
            let response = JSON.parse(data);
            let msg = 'Private API test SUCCEEDED';

            if (response.Success === false)
                msg = 'Private API test FAILED';

            Materialize.toast(msg, 4000);
        })
    }

    function removeProviderKeys(e) {
        let id = inputProviderId.val();

        primeTcpClient.deleteProviderKeys(id, (event, data) => {
            let response = JSON.parse(data);
            let msg = 'Keys successfully deleted';

            if (response.Success === false)
                msg = 'Error during keys deleting';

            Materialize.toast(msg, 4000);
        });
    }

    function saveProviderDetails(e) {
        let id = inputProviderId.val();

        let keys = {
            Key: inputPublicKey.val(),
            Secret: inputPrivateKey.val(),
            Extra: inputExtra.val()
        };

        primeTcpClient.saveProviderKeys(id, keys, (event, data) => {
            let response = JSON.parse(data);
            let msg = 'Keys successfully saved';

            if (response.Success === false)
                msg = 'Error during keys saving';

            Materialize.toast(msg, 4000);
        });
    }

    function showProviderDetails(e) {
        let providerCard = $(e.currentTarget).parents(".card");
        let providerId = providerCard.find(".provider-id").text();
        let providerName = providerCard.find(".card-title").text();
        editProviderModal.find('.title').text("Edit " + providerName + " keys");

        primeTcpClient.getProviderDetails(providerId, (event, data) => {
            let provider = JSON.parse(data);

            inputPrivateKey.val(provider.Secret).focus();
            inputExtra.val(provider.Extra).focus();
            inputProviderId.val(provider.Id);
            inputPublicKey.val(provider.Key).focus();
        });
    }

    function initModal() {
        $('.modal').modal({
            dismissible: true,
        });
    }

    function getPrivateProviders() {
        primeTcpClient.getPrivateProvidersList(function (event, data) {
            providersList = JSON.parse(data);

            displayProviders(providersList);
        });
    }

    function displayProviders(providers) {
        var html = "";

        for (var i = 0; i < providers.length; i++) {
            var provider = providers[i];

            html +=
                `<div class="row">
                    <div class="col s12">
                    <div class="card blue-grey darken-1 ">
                        <div class="card-content white-text">
                            ` + (provider.HasKeys === true ? '<i class="right material-icons has-api-keys-icon">vpn_key</i>' : '') + `
                            <span class="card-title">` + provider.Name + `</span>
                            <p class="italic grey-text text-lighten-3 provider-id">`+ provider.Id + `</p>
                        </div>
                        <div class="card-action">
                            <a class="waves-effect waves-light btn modal-trigger light-green darken-2" href="#editProviderModal">Manage</a>
                        </div>
                    </div>
                    </div>
                </div>`;
        }

        $("#providers-list").html(html);
    }
}

var indexView = new IndexView();
indexView.run();
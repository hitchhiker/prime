import { Logger } from "../utils/Logger";
import { IpcManager } from "../core/IpcManager";
import { PrimeTcpClient } from "../core/PrimeTcpClient";
import $ = require("jquery");
import { ActionThrottler } from "../core/ActionThrottler";

export class IndexView {
    private ipcManager: IpcManager = IpcManager.i();
    private providersList: any[];
    private actionThrottler: ActionThrottler;

    private inputSearchProvider = $('#searchProvider');
    private inputPublicKey = $('#publicKey');
    private inputPrivateKey = $('#privateKey');
    private inputExtra = $('#extra');
    private inputProviderId = $('#providerId');

    private editProviderModal = $('#editProviderModal');

    constructor() {
        this.actionThrottler = new ActionThrottler();
    }

    run() {
        this.inputSearchProvider.on('input', () => {
            this.actionThrottler.throttle(300, () => {
                let searchValue = this.inputSearchProvider.val();
                let filteredProvidersList = this.providersList.filter(p => p.Name.toLowerCase().indexOf(searchValue.toLowerCase()) !== -1);
                this.displayProviders(filteredProvidersList);
            });
        });

        this.editProviderModal = $(document).find('#editProviderModal');

        console.log(this.inputSearchProvider);

        $(document).on("click", ".modal-trigger", (e) => { this.showProviderDetails(e); });
        $(document).on("click", "#saveAndClose", (e) => { this.saveProviderDetails(e); });
        $(document).on("click", "#removeKeys", (e) => { this.removeProviderKeys(e); });
        $(document).on("click", "#testPrivateApi", (e) => { this.testPrivateApi(e); });

        this.getPrivateProviders();
        this.initModal();
    }

    private testPrivateApi(e) {
        let id = this.inputProviderId.val();

        let keys = {
            Key: this.inputPublicKey.val(),
            Secret: this.inputPrivateKey.val(),
            Extra: this.inputExtra.val()
        };

        this.ipcManager.testPrivateApiMessage.call({ id: id, keys: keys }, this.testPrivateApiCall);
    }

    private testPrivateApiCall(event, data) {
        let response = JSON.parse(data);
        let msg = 'Private API test SUCCEEDED';

        if (response.Success === false)
            msg = 'Private API test FAILED';

        Materialize.toast(msg, 4000);
    }

    private removeProviderKeys(e) {
        let id = this.inputProviderId.val();

        this.ipcManager.deleteProviderKeysMessage.call(id, this.deleteProviderKeysMessageCall);
    }

    private deleteProviderKeysMessageCall(event, data) {
        let response = JSON.parse(data);
        let msg = 'Keys successfully deleted';

        if (response.Success === false)
            msg = 'Error during keys deleting';

        Materialize.toast(msg, 4000);
    }

    private saveProviderDetails(e) {
        let id = this.inputProviderId.val();

        let keys = {
            Key: this.inputPublicKey.val(),
            Secret: this.inputPrivateKey.val(),
            Extra: this.inputExtra.val()
        };

        this.ipcManager.saveProviderKeysMessage.call({ id: id, keys: keys }, (event, data) => {
            let response = JSON.parse(data);
            let msg = 'Keys successfully saved';

            if (response.Success === false)
                msg = 'Error during keys saving';

            Materialize.toast(msg, 4000);
        });
    }

    private showProviderDetails(e) {
        let providerCard = $(e.currentTarget).parents(".card");
        let providerId = providerCard.find(".provider-id").text();
        let providerName = providerCard.find(".card-title").text();

        //console.log($('#editProviderModal'));
        this.editProviderModal.find('.title').text("Edit " + providerName + " keys");


        this.ipcManager.getProviderDetailsMessage.call(providerId, (event, data) => {
            let provider = JSON.parse(data);

            this.inputPrivateKey.val(provider.Secret).focus();
            this.inputExtra.val(provider.Extra).focus();
            this.inputProviderId.val(provider.Id);
            this.inputPublicKey.val(provider.Key).focus();
        });
    }

    private getPrivateProviders() {
        this.ipcManager.getPrivateProvidersListMessage.call(null, (event, data) => {
            this.providersList = JSON.parse(data);

            this.displayProviders(this.providersList);
        });
    }

    private initModal() {
        $('.modal').modal({
            dismissible: true,
        });
    }

    private displayProviders(providersList: any[]) {
        var html = "";

        for (var i = 0; i < providersList.length; i++) {
            var provider = providersList[i];

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


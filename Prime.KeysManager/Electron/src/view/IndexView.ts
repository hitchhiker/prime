import { Logger } from "../utils/Logger";
import { IpcManager } from "../core/IpcManager";
import { PrimeTcpClient } from "../core/PrimeTcpClient";
import $ = require("jquery");

export class IndexView {
    private ipcManager: IpcManager = IpcManager.i();
    private providersList: any[];

    run() {
        Logger.log("Index run");
        this.ipcManager.helloMessage.call(JSON.stringify({ age: 22, name: "Alex" }), (event, data) => {
            console.log(data);
        });

        this.getPrivateProviders();
    }

    getPrivateProviders() {
        IpcManager.i().getPrivateProvidersListMessage.call(null, (event, data) => {
            this.providersList = JSON.parse(data);

            this.displayProviders(this.providersList);
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


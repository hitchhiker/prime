import { Logger } from "../utils/Logger";
import Main from "../../Main";
import { ipcMain as IpcMain, WebContents } from "electron";
import { IpcManager } from "./IpcManager";
import { PrimeTcpClient } from "./PrimeTcpClient";
import { IIpcHandlerContext } from "./models/IIpcHandlerContext";

export class KeysManager {
    private dataHandlerChannel: string;
    private primeTcpClient: PrimeTcpClient;

    constructor() {
        this.primeTcpClient = new PrimeTcpClient();
    }

    run() {
        this.primeTcpClient.connect();
        this.primeTcpClient.registerIpc();
        this.registerIpc();
    }

    private registerIpc() {
        // IpcManager.i().getPrivateProvidersListMessage.handle((context: IIpcHandlerContext) => {
        //     Logger.log("Querying providers list...");

        //     this.tcpClient.write(JSON.stringify({
        //         "Type": "PrivateProvidersListMessage"
        //     }));

        //     return "";
        // });

        // IpcMain.on('hello-main', (e, data) => {
        //     let sender: WebContents = e.sender;
        //     console.log(data);

        //     sender.send('hello-renderer', JSON.stringify({main:'Me'}));
        // });

        IpcMain.on('prime:generate-client-guid', (event, arg) => {
            Logger.log("Calling server to generate GUID...");
        
            this.tcpClient.write(JSON.stringify({
                "Type": "GenerateGuidMessage",
            }));
        
            this.dataHandlerChannel = "prime:client-guid-generated";
        });
        
        IpcMain.on('prime:get-private-providers-list', (event, arg) => {
            Logger.log("Querying providers list...");
        
            this.tcpClient.write(JSON.stringify({
                "Type": "PrivateProvidersListMessage"
            }));
            this.dataHandlerChannel = "prime:private-providers-list";
        });
        
        IpcMain.on('prime:get-provider-details', (event, arg) => {
            Logger.log("Querying provider details...");
        
            this.tcpClient.write(JSON.stringify({
                "Type": "ProviderDetailsMessage",
                "Id": arg
            }));
            this.dataHandlerChannel = "prime:provider-details";
        })
        
        IpcMain.on('prime:save-provider-keys', (event, arg) => {
            Logger.log("Saving provider keys...");
        
            this.tcpClient.write(JSON.stringify({
                "Type": "ProviderKeysMessage",
                "Id": arg.id,
                "Key": arg.keys.Key,
                "Secret": arg.keys.Secret,
                "Extra": arg.keys.Extra
            }));
            this.dataHandlerChannel = "prime:provider-keys-saved";
        })
        
        IpcMain.on('prime:delete-provider-keys', (event, arg) => {
            Logger.log("Saving provider keys...");
        
            this.tcpClient.write(JSON.stringify({
                "Type": "DeleteProviderKeysMessage",
                "Id": arg,
            }));
            this.dataHandlerChannel = "prime:provider-keys-deleted";
        });
        
        IpcMain.on('prime:test-private-api', (event, arg) => {
            Logger.log("Testing private API...");
        
            this.tcpClient.write(JSON.stringify({
                "Type": "TestPrivateApiMessage",
                "Id": arg.id,
                "Key": arg.keys.Key,
                "Secret": arg.keys.Secret,
                "Extra": arg.keys.Extra
            }));
            this.dataHandlerChannel = "prime:private-api-tested";
        });
    }
}


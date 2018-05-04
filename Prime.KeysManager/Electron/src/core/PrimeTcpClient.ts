import { IpcManager } from "./IpcManager";
import { Logger } from "../utils/Logger";
import { TcpClient } from "../transport/TcpClient";
import Main from "../../Main";
import { IIpcHandlerContext } from "./models/IIpcHandlerContext";
import { IIpcMessage } from "./msgs/IIpcMessage";

// NodeJS client. Should be used in Main process of Electron.
export class PrimeTcpClient {
    private tcpClient: TcpClient;
    private responseBuffer: any[];
    private dataHandlerChannel: string;

    private tcpClientDataReceivedCallbackMessage: IIpcMessage = null;

    connect() {
        Logger.log("Starting TCP client...");

        this.tcpClient = new TcpClient();

        this.tcpClient.onClientConnected = () => {
            Logger.log("Connected to Prime API server.");
        };
        this.tcpClient.onDataReceived = (data: any) => {
            if (this.tcpClientDataReceivedCallbackMessage !== null) {
                this.tcpClientDataReceivedCallbackMessage.callBackLast(data);
                Logger.log("Client received data. Sending to last channel...");
            } else {
                Logger.log("tcpClientDataReceivedCallbackMessage is null.");
            }
        };
        this.tcpClient.onConnectionClosed = () => {
            Logger.log("Connection closed.");
        };

        this.tcpClient.connect(19991, '127.0.0.1');
    }

    registerIpc() {
        IpcManager.i().getPrivateProvidersListMessage.handle((context: IIpcHandlerContext) => {
            Logger.log("Querying providers list...");

            this.tcpClient.write(JSON.stringify({
                "Type": "PrivateProvidersListMessage"
            }));

            console.log(context);

            this.tcpClientDataReceivedCallbackMessage = context.ipcMessageCaller;

            return null;
        })
    }

    getPrivateProvidersList(callback: (event, data) => void) {
        IpcManager.i().getPrivateProvidersListMessage.call(null, callback);
    }

    getProviderDetails() {

    }
}

import { Injectable, Inject } from '@angular/core';
import { SocketClient } from '../models/interfaces/socket-client';
import { LoggerService } from './logger.service';
import { SocketState } from '../models/socket-state';
import { ProvidersListRequestMessage, BaseMessage, TestPrivateApiRequestMessage, ProvidersListResponseMessage, ProviderDetailsResponseMessage, ProviderDetailsRequestMessage, ProviderSaveKeysRequestMessage, ProviderSaveKeysResponseMessage, PrivateProvidersListResponseMessage, PrivateProvidersListRequestMessage, DeleteProviderKeysResponseMessage, DeleteProviderKeysRequestMessage, TestPrivateApiResponseMessage, ProviderHasKeysResponseMessage, ProviderHasKeysRequestMessage, BaseRequestMessage, BaseResponseMessage, GetSupportedMarketsRequestMessage, GetSupportedMarketsResponseMessage, DownloadFileResponseMessage, DownloadFileRequestMessage } from '../models/messages';
import { PrivateApiContext } from '../models/private-api-context';
import { ExchangeDetails } from '../models/exchange-details';
import { Subscription, Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class PrimeSocketService {
    private socketClient: SocketClient;
    
    constructor() { }

    private responseBuffer: any[];
    private dataHandlerChannel: string;

    private callbacks: ((data) => void)[] = [];

    // TODO: implement multiple subscriptions.
    onClientConnected: Subject<any> = new Subject<any>();
    onConnectionClosed: Subject<any> = new Subject<any>();
    onErrorOccurred: Subject<any> = new Subject<any>();

    socketState: SocketState = SocketState.Disconnected;

    // Connection.

    connect() {
        LoggerService.log("Starting TCP client...");

        this.socketClient.onClientConnected = () => {
            LoggerService.log("Connected to Prime API server.");
            this.socketState = SocketState.Connected;

            this.onClientConnected.next();
        };

        this.socketClient.onDataReceived = (data: any) => {
            var response: BaseResponseMessage = JSON.parse(data.data);
            // console.log("* Current handlers: ");
            // console.log(this.callbacks);
            // console.log("* Response: " + response.$type);

            if (this.callbacks[response.$type] != undefined) {
                this.callbacks[response.$type](response);
                delete this.callbacks[response.$type];
                // console.log("* Deleted: " + response.$type);
            } else {
                throw "Callback method is not found for " + response.$type;
            }
        };

        this.socketClient.onConnectionClosed = () => {
            LoggerService.log("Connection closed.");
            this.socketState = SocketState.Disconnected;

            this.onConnectionClosed.next();
        };

        this.socketClient.onErrorOccurred = () => {
            this.onErrorOccurred.next();
        };

        this.socketClient.connect('ws://127.0.0.1:9992/');
    }

    // Socket messaging.

    private writeSocket(data: string, callback?: (data) => void) {
        this.socketClient.write(data);
    }

    private writeSocketMessage(data: BaseRequestMessage, callback?: (data) => void) {
        this.callbacks[data.expectedEmptyResponse.$type] = callback;
        
        this.writeSocket(data.serialize(), callback);
    }

    // Core logic methods.

    test() {
        this.writeSocket("Hello");
    }

    downloadFile(callback: (data: DownloadFileResponseMessage) => void) {
        this.writeSocketMessage(new DownloadFileRequestMessage(), callback);
    }

    getPrivateProvidersList(callback: (data: PrivateProvidersListResponseMessage) => void) {
        let x = new PrivateProvidersListRequestMessage();

        this.writeSocketMessage(new PrivateProvidersListRequestMessage(), callback);
    }

    getProviderDetails(idHash: string, callback: (data: ProviderDetailsResponseMessage) => void) {
        this.writeSocketMessage(new ProviderDetailsRequestMessage(idHash), callback);
    }

    checkProvidersKeys(idHash: string, callback: (data: ProviderHasKeysResponseMessage) => void) {
        let msg = new ProviderHasKeysRequestMessage();
        msg.id = idHash;

        this.writeSocketMessage(msg, callback);
    }

    saveApiKeys(exchangeDetails: ExchangeDetails, callback: (data: ProviderSaveKeysResponseMessage) => void) {
        let msg = new ProviderSaveKeysRequestMessage();
        msg.id = exchangeDetails.privateApiContext.exchangeId;
        msg.key = exchangeDetails.privateApiContext.key;
        msg.secret = exchangeDetails.privateApiContext.secret;
        msg.extra = exchangeDetails.privateApiContext.extra;

        this.writeSocketMessage(msg, callback);
    }

    deleteKeys(exchangeId: string, callback: (data: DeleteProviderKeysResponseMessage) => void) {
        let msg = new DeleteProviderKeysRequestMessage();
        msg.id = exchangeId;

        this.writeSocketMessage(msg, callback);
    }

    testPrivateApi(privateApiContext: PrivateApiContext, callback: (data: TestPrivateApiResponseMessage) => void) {
        var msg = new TestPrivateApiRequestMessage();
        msg.id = privateApiContext.exchangeId;
        msg.key = privateApiContext.key;
        msg.secret = privateApiContext.secret;
        msg.extra = privateApiContext.extra;

        this.writeSocketMessage(msg, callback);
    }

    getSupportedMarkets(callback: (data: GetSupportedMarketsResponseMessage) => void) {
        var msg = new GetSupportedMarketsRequestMessage();

        this.writeSocketMessage(msg, callback);
    }
}

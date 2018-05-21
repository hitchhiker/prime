import { Injectable, Inject } from '@angular/core';
import { ISocketClient } from '../models/interfaces/ISocketClient';
import { LoggerService } from './logger.service';
import { SocketState } from '../models/socket-state';
import { ProvidersListRequestMessage, BaseMessage, TestPrivateApiRequestMessage, ProvidersListResponseMessage, ProviderDetailsResponseMessage, ProviderDetailsRequestMessage, ProviderSaveKeysRequestMessage, ProviderSaveKeysResponseMessage, PrivateProvidersListResponseMessage, PrivateProvidersListRequestMessage, DeleteProviderKeysResponseMessage, DeleteProviderKeysRequestMessage, TestPrivateApiResponseMessage } from '../models/messages';
import { PrivateApiContext } from '../models/private-api-context';
import { GetProviderDetailsMessage } from 'models/core/msgs/Messages';
import { ExchangeDetails } from '../models/ExchangeDetails';

@Injectable({
    providedIn: 'root'
})
export class PrimeSocketService {

    constructor(@Inject("ISocketClient") private socketClient: ISocketClient) { }

    private responseBuffer: any[];
    private dataHandlerChannel: string;

    private lastCallback: (data) => void = null;

    // TODO: implement multiple subscriptions.
    onClientConnected: () => void = null;
    onConnectionClosed: () => void = null;
    onErrorOccurred: () => void = null;

    socketState: SocketState = SocketState.Disconnected;

    connect() {
        LoggerService.log("Starting TCP client...");

        this.socketClient.onClientConnected = () => {
            LoggerService.log("Connected to Prime API server.");
            this.socketState = SocketState.Connected;

            if (this.onClientConnected !== null) {
                this.onClientConnected();
            }
        };

        this.socketClient.onDataReceived = (data: any) => {
            var objectData = JSON.parse(data.data);

            if (this.lastCallback !== null) {
                this.lastCallback(objectData);
            }
        };

        this.socketClient.onConnectionClosed = () => {
            LoggerService.log("Connection closed.");
            this.socketState = SocketState.Disconnected;

            if (this.onConnectionClosed !== null) {
                this.onConnectionClosed();
            }
        };

        this.socketClient.onErrorOccurred = () => {
            if (this.onErrorOccurred !== null) {
                this.onErrorOccurred();
            }
        };

        this.socketClient.connect('ws://127.0.0.1:9991/');
    }

    private writeSocket(data: string, callback?: (data) => void) {
        if (callback !== null) {
            this.lastCallback = callback;
        }

        this.socketClient.write(data);
    }

    private writeSocketMessage(data: BaseMessage, callback?: (data) => void) {
        this.writeSocket(data.serialize(), callback);
    }

    test() {
        this.writeSocket("Hello");
    }

    getPrivateProvidersList(callback: (data: PrivateProvidersListResponseMessage) => void) {
        this.writeSocketMessage(new PrivateProvidersListRequestMessage(), callback);
    }

    getProviderDetails(idHash: string, callback: (data: ProviderDetailsResponseMessage) => void) {
        this.writeSocketMessage(new ProviderDetailsRequestMessage(idHash), callback);
    }

    saveApiKeys(exchangeDetails: ExchangeDetails, callback: (data: ProviderSaveKeysResponseMessage) => void) {
        let msg = new ProviderSaveKeysRequestMessage();
        msg.id = exchangeDetails.privateApiContext.exchangeId,
            msg.key = exchangeDetails.privateApiContext.key,
            msg.secret = exchangeDetails.privateApiContext.secret,
            msg.extra = exchangeDetails.privateApiContext.extra

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
        msg.extra = privateApiContext.secret

        this.writeSocketMessage(msg, callback);
    }
}

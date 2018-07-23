import { Injectable, Inject } from '@angular/core';
import { ISocketClient } from '../models/interfaces/ISocketClient';
import { LoggerService } from './logger.service';
import { SocketState } from '../models/socket-state';
import { BaseMessage, BaseRequestMessage, BaseResponseMessage } from '../models/messages/BaseMessages';
import { Subscription, Subject } from 'rxjs';
import { UpdateTimeKindRequestMessage, UpdateTimeKindResponseMessage } from '../models/messages/UpdateTimeKindMesseges';

@Injectable({
    providedIn: 'root'
})
export class PrimeSocketService {

    constructor(@Inject("ISocketClient") private socketClient: ISocketClient) { }

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

    updateTimeKind(isUtc: boolean, callback: (data: UpdateTimeKindResponseMessage) => void) {
        var msg = new UpdateTimeKindRequestMessage();
        msg.isUtc = isUtc; 

        this.writeSocketMessage(msg, callback);
    }
}

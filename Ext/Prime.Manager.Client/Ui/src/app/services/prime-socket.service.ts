import { Injectable, Inject } from '@angular/core';
import { ISocketClient } from '../models/interfaces/ISocketClient';
import { LoggerService } from './logger.service';
import { SocketState } from '../models/socket-state';
import { ProvidersListRequestMessage, BaseMessage, TestPrivateApiMessageRequest, ProvidersListResponseMessage, ProviderDetailsResponseMessage, ProviderDetailsRequestMessage } from '../models/messages';
import { PrivateApiContext } from '../models/private-api-context';
import { GetProviderDetailsMessage } from 'models/core/msgs/Messages';

@Injectable({
  providedIn: 'root'
})
export class PrimeSocketService {

  constructor(@Inject("ISocketClient") private socketClient: ISocketClient) { }

  private responseBuffer: any[];
  private dataHandlerChannel: string;

  private lastCallback: (data) => void = null;

  socketState: SocketState = SocketState.Disconnected;

  connect(callback: () => void) {
    LoggerService.log("Starting TCP client...");

    this.socketClient.onClientConnected = () => {
      LoggerService.log("Connected to Prime API server.");
      this.socketState = SocketState.Connected;
      callback();
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

  getProviderProvidersList(callback: (data: ProvidersListResponseMessage) => void) {
    this.writeSocketMessage(new ProvidersListRequestMessage(), callback);
  }

  getProviderDetails(idHash: string, callback: (data: ProviderDetailsResponseMessage) => void) {
    this.writeSocketMessage(new ProviderDetailsRequestMessage(idHash), callback);
  }

  testPrivateApi(privateApiContext: PrivateApiContext) {
    this.writeSocketMessage(new TestPrivateApiMessageRequest(
      privateApiContext.exchangeId,
      privateApiContext.key,
      privateApiContext.secret,
      privateApiContext.extra
    ));
  }
}

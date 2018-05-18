import { Injectable } from '@angular/core';
import { ISocketClient } from '../models/interfaces/ISocketClient';
import { LoggerService } from './logger.service';

@Injectable({
  providedIn: 'root'
})
export class WsClientService implements ISocketClient {
  ws: WebSocket;

  constructor() { 
    
  }

  connect(host: string) {
    this.ws = new WebSocket(host);
    this.ws.onopen = this.onClientConnected;
    this.ws.onmessage = this.onDataReceived;
    this.ws.onclose = this.onConnectionClosed;
  }

  write(data: string) {
    this.ws.send(data);
  }

  onClientConnected: () => void;
  onDataReceived: (data: any) => void;
  onConnectionClosed: () => void;  
}

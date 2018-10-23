import { Injectable } from '@angular/core';
import { SocketClient } from '../models/interfaces/socket-client';

@Injectable({
  providedIn: 'root'
})
export class WsClientService implements SocketClient {
  ws: WebSocket;

  constructor() { 
    
  }

  connect(host: string) {
    this.ws = new WebSocket(host);
    this.ws.onopen = this.onClientConnected;
    this.ws.onmessage = this.onDataReceived;
    this.ws.onclose = this.onConnectionClosed;
    this.ws.onerror = this.onErrorOccurred;
  }

  write(data: string) {
    this.ws.send(data);
  }

  onClientConnected: () => void;
  onDataReceived: (data: any) => void;
  onConnectionClosed: () => void;  
  onErrorOccurred: () => void;
}

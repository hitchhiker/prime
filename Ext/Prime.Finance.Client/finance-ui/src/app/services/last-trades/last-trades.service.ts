import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { WebsocketService } from '../websocket.service';
import { LastTradeRespose } from './bitfinex/last-trade-response';
import { RemoteResponse } from '../../models/remote-response';
import { WsDataService } from '../ws-data.service';

@Injectable({
  providedIn: 'root'
})
export class LastTradesService extends WsDataService {
  protected endpointURL: string = "wss://api.bitfinex.com/ws/2";
  
  private _lastTrades: BehaviorSubject<RemoteResponse> = new BehaviorSubject(null);
  public readonly lastTrades: Observable<RemoteResponse> = this._lastTrades.asObservable();

  constructor(
    ws: WebsocketService
  ) {
    super(ws);
  }

  public connect() {
    this.connectToEndpoint();

    this.wsOnConnected.subscribe((msg: MessageEvent) => {
      this.onConnected();
    });

    this.wsOnMessage.subscribe((msg: MessageEvent) => {
      this.onMessage(msg);
    });
  }

  private onConnected() {
    // Start getting latest trades.
    this.sendMessage({
      event: 'subscribe',
      channel: 'trades',
      symbol: 'tBTCUSD'
    });

    console.log("Connected to Bitfinex last trades");
  }

  private onMessage(msg: MessageEvent) {
    // Messages parsing.
    let rRaw = JSON.parse(msg.data);
    let lastTrades = new LastTradeRespose(rRaw).toRemoteResponse();

    this._lastTrades.next(lastTrades);
  }
}

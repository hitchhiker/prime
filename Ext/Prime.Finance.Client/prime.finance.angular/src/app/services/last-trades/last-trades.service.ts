import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { WebsocketService } from '../websocket.service';
import { LastTradeRespose } from './bitfinex/last-trade-response';
import { RemoteResponse } from '../../models/remote-response';
import { WsBitfinexDataServiceBase } from '../ws-bitfinex-data-base.service';

@Injectable({
  providedIn: 'root'
})
export class LastTradesService extends WsBitfinexDataServiceBase {
  public onConnectedHandler(msgEvent: MessageEvent) {
    // Start getting latest trades.
    this.sendMessage({
      event: 'subscribe',
      channel: 'trades',
      symbol: 'tBTCUSD'
    });

    console.log("Connected to Bitfinex last trades");
  }

  public onMessageHandler(msgEvent: MessageEvent) {
    // Messages parsing.
    let rRaw = JSON.parse(msgEvent.data);
    let lastTrades = new LastTradeRespose(rRaw).parseResponse();

    this._lastTrades.next(lastTrades);
  }

  private _lastTrades: BehaviorSubject<RemoteResponse> = new BehaviorSubject(null);
  public readonly lastTrades: Observable<RemoteResponse> = this._lastTrades.asObservable();

  constructor(
    ws: WebsocketService
  ) {
    super(ws);
  }
}

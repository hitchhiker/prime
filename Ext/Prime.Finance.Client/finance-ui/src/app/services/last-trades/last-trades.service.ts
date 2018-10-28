import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { LastTrade } from 'src/models/view/last-trade';
import { OrderSide } from 'src/app/models/trading/order-side';
import { WebsocketService } from '../websocket.service';
import { map } from 'rxjs/operators';
import { LastTradeRespose } from './bitfinex/last-trade-response';
import { TradeInfo } from './bitfinex/trade-info';
import { ResponseType } from './bitfinex/response-type';
import { WsDataService } from '../ws-data.service';
import { getLocaleDateTimeFormat } from '@angular/common';
import { DateTime } from './date-time';

@Injectable({
  providedIn: 'root'
})
export class LastTradesService extends WsDataService {
  private _lastTrades: BehaviorSubject<TradeInfo> = new BehaviorSubject(null);
  public readonly lastTrades: Observable<TradeInfo> = this._lastTrades.asObservable();

  constructor(
    ws: WebsocketService
  ) {
    super("wss://api.bitfinex.com/ws/2", ws);
  }

  public connect() {
    this.connectToEndpoint();

    this.wsOnConnected.subscribe((msg: MessageEvent) => {
      this.onConnected(msg);
    });

    this.wsOnMessage.subscribe((msg: MessageEvent) => {
      this.onMessage(msg);
    });
  }

  private onConnected(msg: MessageEvent) {
    console.log("Connected to Bitfinex last trades");

    // Start getting latest trades.
    this.sendMessage({
      event: 'subscribe',
      channel: 'trades',
      symbol: 'tBTCUSD'
    });
  }

  private onMessage(msg: MessageEvent) {
    // Messages parsing.
    let rRaw = JSON.parse(msg.data);
    let lastTrades = LastTradeRespose.fromRawResponse(rRaw);

    this._lastTrades.next(lastTrades);
  }
}

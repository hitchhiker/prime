import { Injectable } from '@angular/core';
import { WsBitfinexDataServiceBase } from '../ws-bitfinex-data-base.service';
import { WebsocketService } from '../websocket.service';
import { Subject, BehaviorSubject, Observable } from 'rxjs';
import { RemoteResponse } from 'src/app/models/remote-response';
import { OrderbookResponse } from './bitfinex/orderbook-response';

@Injectable({
  providedIn: 'root'
})
export class OrderbookService extends WsBitfinexDataServiceBase {
  public onConnectedHandler(msgEvent: MessageEvent) {
    // Start getting latest trades.
    this.sendMessage({
      "event": "subscribe",
      "channel": "book",
      "pair": "BTCUSD",
      "prec": "R0"
    });

    console.log("Connected to Bitfinex orderbook");
  }
  public onMessageHandler(msgEvent: MessageEvent) {
    let rRaw = JSON.parse(msgEvent.data);

    if (rRaw.event && (rRaw.event === "info" || rRaw.event === "subscribed"))
      return;

    let orderbookResponse = new OrderbookResponse(rRaw);
    let response = orderbookResponse.parseResponse();

    this._orderbookSubject.next(response);
  }

  private _orderbookSubject: BehaviorSubject<RemoteResponse> = new BehaviorSubject<RemoteResponse>(null);
  public orderbook: Observable<RemoteResponse> = this._orderbookSubject.asObservable();

  constructor(
    ws: WebsocketService
  ) {
    super(ws);
  }
}

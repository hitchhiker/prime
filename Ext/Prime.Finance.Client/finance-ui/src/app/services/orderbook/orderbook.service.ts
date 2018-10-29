import { Injectable } from '@angular/core';
import { WsDataService } from '../ws-data.service';
import { WebsocketService } from '../websocket.service';
import { Subject, BehaviorSubject, Observable } from 'rxjs';
import { RemoteResponse } from 'src/app/models/remote-response';
import { OrderbookResponse } from './bitfinex/orderbook-response';

@Injectable({
  providedIn: 'root'
})
export class OrderbookService extends WsDataService {
  protected endpointURL: string = "wss://api.bitfinex.com/ws/2";

  private _orderbookSubject: BehaviorSubject<RemoteResponse> = new BehaviorSubject<RemoteResponse>(null);
  public orderbook: Observable<RemoteResponse> = this._orderbookSubject.asObservable();

  constructor(
    ws: WebsocketService
  ) {
    super(ws);
  }

  public connect() {
    this.connectToEndpoint();

    this.wsOnConnected.subscribe((msg) => {
      this.onConnected(msg);
    });

    this.wsOnMessage.subscribe((msg) => {
      this.onMessage(msg);
    });
  }

  private onMessage(msg: MessageEvent): any {
    let rRaw = JSON.parse(msg.data);

    if (rRaw.event && (rRaw.event === "info" || rRaw.event === "subscribed"))
      return;

    let orderbookResponse = new OrderbookResponse(rRaw);
    let response = orderbookResponse.parseResponse();

    this._orderbookSubject.next(response);
  }

  private onConnected(msg: MessageEvent): any {
    // Start getting latest trades.
    this.sendMessage({
      "event": "subscribe",
      "channel": "book",
      "pair": "BTCUSD",
      "prec": "R0"
    });

    console.log("Connected to Bitfinex orderbook");
  }
}

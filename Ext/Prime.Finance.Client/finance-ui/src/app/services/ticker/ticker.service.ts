import { Injectable } from '@angular/core';
import { WsBitfinexDataServiceBase } from '../ws-bitfinex-data-base.service';
import { WebsocketService } from '../websocket.service';
import { Subject, Observable } from 'rxjs';
import { TickerResponse } from './bitfinex/ticker-response';
import { RemoteResponse } from 'src/app/models/remote-response';

@Injectable({
  providedIn: 'root'
})
export class TickerService extends WsBitfinexDataServiceBase {
  public onConnectedHandler(msgEvent: MessageEvent) {
    console.log("Connected to Bitfinex ticker");

    // Start receiving ticker data.
    this.sendMessage({
      "event": "subscribe",
      "channel": "ticker",
      "pair": "BTCUSD"
    });
  }

  public onMessageHandler(msgEvent: MessageEvent) {
    let rRaw = JSON.parse(msgEvent.data);

    let ticker = new TickerResponse(rRaw).parseResponse();

    if (ticker !== null) {
      this._ticker.next(ticker);
    }
  }

  private _ticker: Subject<RemoteResponse> = new Subject();
  public ticker: Observable<RemoteResponse> = this._ticker.asObservable();

  constructor(
    ws: WebsocketService
  ) {
    super(ws);
  }
}

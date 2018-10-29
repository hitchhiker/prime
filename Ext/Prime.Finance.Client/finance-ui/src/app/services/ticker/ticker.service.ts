import { Injectable } from '@angular/core';
import { WsDataService } from '../ws-data.service';
import { WebsocketService } from '../websocket.service';
import { Subject, Observable } from 'rxjs';
import { TickerResponse } from './bitfinex/ticker-response';
import { RemoteResponse } from 'src/app/models/remote-response';

@Injectable({
  providedIn: 'root'
})
export class TickerService extends WsDataService {
  protected endpointURL: string = "wss://api.bitfinex.com/ws/2";

  private _ticker: Subject<RemoteResponse> = new Subject();
  public ticker: Observable<RemoteResponse> = this._ticker.asObservable();

  public connect() {
    this.connectToEndpoint();

    this.wsOnConnected.subscribe((msg) => {
      this.onConnected(msg);
    });

    this.wsOnMessage.subscribe((msg) => {
      this.onMessage(msg);
    });
  }

  private onConnected(msg: MessageEvent): any {
    // Start receiving ticker data.
    this.sendMessage({
      "event": "subscribe",
      "channel": "ticker",
      "pair": "BTCUSD"
    });
  }

  private onMessage(msg: MessageEvent): any {
    let rRaw = JSON.parse(msg.data);
    console.log(rRaw);
    
    let ticker = new TickerResponse(rRaw).parseResponse();

    if (ticker !== null) {
      this._ticker.next(ticker);
    }
  }

  constructor(
    ws: WebsocketService
  ) {
    super(ws);
  }
}

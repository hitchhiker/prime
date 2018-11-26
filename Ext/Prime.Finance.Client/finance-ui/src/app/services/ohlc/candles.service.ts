import { Injectable } from '@angular/core';
import { WsBitfinexDataServiceBase } from '../ws-bitfinex-data-base.service';
import { WebsocketService } from '../websocket.service';

@Injectable({
  providedIn: 'root'
})
export class CandlesService extends WsBitfinexDataServiceBase {

  public onConnectedHandler(msgEvent: MessageEvent) {
    // Start getting latest trades.
    this.sendMessage({
      event: 'subscribe',
      channel: 'candles',
      key: 'trade:1m:tBTCUSD'
    });
    console.log("Connected to Bitfinex candles.");
  }

  public onMessageHandler(msgEvent: MessageEvent) {
    // Messages parsing.
    let rRaw = JSON.parse(msgEvent.data);
    console.log(rRaw);
  }

  constructor(
    ws: WebsocketService
  ) {
    super(ws);
  }
}

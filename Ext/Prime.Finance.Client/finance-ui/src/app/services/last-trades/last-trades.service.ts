import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { LastTrade } from 'src/models/view/last-trade';
import { OrderSide } from 'src/app/models/trading/order-side';
import { WebsocketService } from '../websocket.service';
import { map } from 'rxjs/operators';
import { LastTradeRespose } from './bitfinex/last-trade-response';
import { TradeInfo } from './bitfinex/trade-info';
import { ResponseType } from './bitfinex/response-type';

@Injectable({
  providedIn: 'root'
})
export class LastTradesService {
  private _lastTrades: BehaviorSubject<TradeInfo> = new BehaviorSubject(null);
  public readonly lastTrades: Observable<TradeInfo> = this._lastTrades.asObservable();

  public lastTradesObs: Observable<any>;

  private static readonly endpointUrl = "wss://api.bitfinex.com/ws/2";

  private wsSubject: Subject<any>;

  constructor(
    private ws: WebsocketService
  ) {
    // this._lastTrades.next([
    //   { Date: "14:52:32", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
    //   { Date: "14:52:33", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
    //   { Date: "14:52:34", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
    //   { Date: "14:52:35", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
    //   { Date: "14:52:36", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 }
    // ]);
  }

  public connect() {
    this.wsSubject = <Subject<any>>this.ws.connect(LastTradesService.endpointUrl)
      .pipe(map((msg, ind): MessageEvent => {
        return msg;
      }));
    this.wsSubject.subscribe((msg: MessageEvent) => {
      if (msg.type === "open") {
        this.onConnected(msg);
      } else if (msg.type === "message") {
        this.onMessage(msg);
      }
    });
  }

  private onConnected(msg: MessageEvent) {
    console.log("Connected to Bitfinex last trades");

    this.wsSubject.next({
      event: 'subscribe',
      channel: 'trades',
      symbol: 'tBTCUSD'
    });
  }

  private unitToTimeString(timestamp: number): string {
    var date = new Date(timestamp);
    // Hours part from the timestamp
    var hours = date.getHours();
    // Minutes part from the timestamp
    var minutes = "0" + date.getMinutes();
    // Seconds part from the timestamp
    var seconds = "0" + date.getSeconds();
    
    // Will display time in 10:30:23 format
    return hours + ':' + minutes.substr(-2) + ':' + seconds.substr(-2);
  }

  private onMessage(msg: MessageEvent) {
    let arr: any[] = JSON.parse(msg.data);

    if (arr.length === 2) {
      // Snapshot or heartbeat.
      if (typeof arr[1] == "string") {
        // Heartbeat.
      } else {
        // Snapshot.

        // TODO: reimplement.
        arr = arr.sort((a, b) => {
          return a[1] >= b[1] ? 1: 0;
        });

        // Get LastTrade objects array.
        let snapshotTrades: Array<LastTrade> = [];
        for (let i = 0; i < arr[1].length; i++) {
          const r = LastTradeRespose.fromArray(arr[1][i]);
          snapshotTrades.push(
            {
              Id: r.ID,
              Amount: Math.abs(r.AMOUNT),
              Date: this.unitToTimeString(r.MTS),
              Price: r.PRICE,
              Type: r.AMOUNT >= 0 ? OrderSide.Buy : OrderSide.Sell
            }
          )
        }

        // Notify.
        this._lastTrades.next({
          responseType: ResponseType.Snapshot,
          payload: snapshotTrades
        });
      }
    }
    else if (arr.length === 3) {
      // Updated.
      if(arr[1] === "tu") {
        // Notify.
        let r = LastTradeRespose.fromArray(arr[2]);

        this._lastTrades.next({
          responseType: ResponseType.TransactionUpdated,
          payload: {
            Id: r.ID,
            Amount: Math.abs(r.AMOUNT),
            Date: this.unitToTimeString(r.MTS),
            Price: r.PRICE,
            Type: r.AMOUNT >= 0 ? OrderSide.Buy : OrderSide.Sell
          }
        });
      }
    }

    // for (let i = 0; i < obj[1].length; i++) {
    //   const r = obj[i];
    //   console.log(r);
    // }

    //console.log(LastTradeRespose.fromArray(obj));
  }

  public addLastTrade(lastTrade: LastTrade) {
    //this._lastTrades.getValue().push(lastTrade);
    //this._lastTrades.next(this._lastTrades.getValue());
  }
}

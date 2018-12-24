import { Component, OnInit } from '@angular/core';
import { PrimeSocketService } from 'src/app/services/prime-socket.service';
import { OrderContext } from 'src/app/models/trading/order-context';
import { OrderSide } from 'src/app/models/trading/order-side';
import { OrderType } from 'src/app/models/trading/order-type';
import { AppService } from 'src/app/services/app.service';
import { Asset } from '../models/trading/asset';
import { AssetPair } from '../models/trading/asset-pair';
import { LastTradesService } from '../services/last-trades/last-trades.service';
import { WebsocketService } from '../services/websocket.service';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-trading-panel',
  templateUrl: './trading-panel.component.html',
  styleUrls: ['./trading-panel.component.scss']
})
export class TradingPanelComponent implements OnInit {

  constructor(
    private app: AppService,
    private lastTradesService: LastTradesService,
    private websocket: WebsocketService,
    private primeClient: PrimeSocketService) { }

  selectedTabLimit: boolean = true;
  selectedTabMarket: boolean = false;

  selectTabMarket(): boolean {
    this.deselectAllTabs();

    this.selectedTabMarket = true;

    return false;
  }

  selectTabLimit(): boolean {
    this.deselectAllTabs();

    this.selectedTabLimit = true;

    return false;
  }

  private deselectAllTabs() {
    this.selectedTabLimit = false;
    this.selectedTabMarket = false;
  }

  ngOnInit() {
  }

  public placeOrderBuyLimit() {
    let orderCtx: OrderContext = {
      side: OrderSide.Buy,
      type: OrderType.Limit,
      market: this.app.state.market,
      amount: 100,
      price: 8000
    }

    this.app.state.setMarket(AssetPair.fromAssetCodes("ETH", "UAH"));

    console.log(orderCtx);
    //this.primeClient.buyLimit();
  }

  public placeOrderSellLimit() {
    let msgs = <Subject<any>>this.websocket
      .connect("wss://api.bitfinex.com/ws/2")
      //.connect("ws://127.0.0.1:9992/")
      .pipe(map((response: MessageEvent): MessageEvent => {
        return response.data;
      }));

    msgs.subscribe((e: MessageEvent) => {
      console.log(e);

    });

    setTimeout(() => {
      msgs.next({
        event: 'subscribe',
        channel: 'trades',
        symbol: 'tBTCUSD'
      });
    }, 1000);
  }
}

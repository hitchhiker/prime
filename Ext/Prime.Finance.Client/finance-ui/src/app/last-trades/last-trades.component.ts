import { Component, OnInit } from '@angular/core';
import { LastTrade } from 'src/models/view/last-trade';
import { OrderSide } from 'src/app/models/trading/order-side';
import { Observable } from 'rxjs';
import { LastTradesService } from 'src/app/services/last-trades/last-trades.service';
import { ResponseType } from '../services/last-trades/bitfinex/response-type';

@Component({
  selector: 'app-last-trades',
  templateUrl: './last-trades.component.html',
  styleUrls: ['./last-trades.component.scss']
})
export class LastTradesComponent implements OnInit {

  constructor(
    public lastTradesService: LastTradesService
  ) { }

  public lastTrades: LastTrade[];

  // lastTrades: LastTrade[] = [
  //   { Date: "14:52:32", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:52:32", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:52:32", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:52:32", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:52:32", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:52:32", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:52:32", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:52:32", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:52:32", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:52:32", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:52:32", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:52:32", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Sell, Price: 7600, Amount: 1.3 },
  //   { Date: "14:45:23", Type: OrderSide.Buy, Price: 7600, Amount: 1.3 },
  // ];

  ngOnInit() {
    this.lastTradesService.connect();
    this.lastTradesService.lastTrades.subscribe((data) => {
      if (data) {
        if (data.responseType === ResponseType.Snapshot) {
          console.log(data.payload);
          this.lastTrades = <LastTrade[]>data.payload;
        }
        else if(data.responseType === ResponseType.TransactionUpdated) {
          this.lastTrades.unshift(data.payload);
        }
      }
    });
  }

}

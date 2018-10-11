import { Component, OnInit } from '@angular/core';
import { LastTrade } from 'src/models/view/last-trade';
import { OrderType } from '../../models/order-type';

@Component({
  selector: 'app-last-trades',
  templateUrl: './last-trades.component.html',
  styleUrls: ['./last-trades.component.scss']
})
export class LastTradesComponent implements OnInit {

  constructor() { }

  lastTrades: LastTrade[] = [
    { Date: "14:52:32", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:52:32", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:52:32", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:52:32", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:52:32", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:52:32", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:52:32", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:52:32", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:52:32", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:52:32", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:52:32", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:52:32", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Sell, Price: 7600, Amount: 1.3 },
    { Date: "14:45:23", Type: OrderType.Buy, Price: 7600, Amount: 1.3 },
  ];

  ngOnInit() {
  }

}

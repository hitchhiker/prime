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
    private lastTradesService: LastTradesService
  ) { 
    console.log(lastTradesService);
    
    lastTradesService.test();

  }

  public lastTrades: LastTrade[];
  private static readonly maxNumberOfLastTrades = 60;

  ngOnInit() {
    

    this.lastTradesService.lastTrades.subscribe((data) => {
      if (data) {
        if (data.responseType === ResponseType.Snapshot) {
          this.lastTrades = data.payload;
        }
        else if(data.responseType === ResponseType.TransactionUpdated) {
          let newLastTrade = <LastTrade>data.payload;
          this.addLastTrade(newLastTrade);
        }
      }
    });
  }

  private addLastTrade(item: LastTrade) {
    this.lastTrades.unshift(item);
    if(this.lastTrades.length >= LastTradesComponent.maxNumberOfLastTrades) {
      this.lastTrades.length = LastTradesComponent.maxNumberOfLastTrades;
    }
  }
}

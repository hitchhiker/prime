import { Component, OnInit } from '@angular/core';
import { AppService } from '../services/app.service';
import { TickerService } from '../services/ticker/ticker.service';
import { Ticker } from '../services/ticker/bitfinex/ticker';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  get ticker(): string {
    return this.app.state.market.tickerUi;
  }

  public latestPrice: number = 0;

  constructor(
    private app: AppService,
    private tickerService: TickerService
  ) { }

  ngOnInit() {
    this.tickerService.connect();
    this.tickerService.ticker.subscribe((r) => {
      let ticker = <Ticker>r.payload;

      this.latestPrice = ticker.lastPrice;
    });
  }
}

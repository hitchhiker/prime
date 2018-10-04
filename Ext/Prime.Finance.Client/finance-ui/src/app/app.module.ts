import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { ChartComponent } from './chart/chart.component';
import { TickersComponent } from './tickers/tickers.component';
import { NavbarComponent } from './navbar/navbar.component';
import { LastTradesComponent } from './last-trades/last-trades.component';
import { OrderbookComponent } from './orderbook/orderbook.component';
import { TradingPanelComponent } from './trading-panel/trading-panel.component';

@NgModule({
  declarations: [
    AppComponent,
    ChartComponent,
    TickersComponent,
    NavbarComponent,
    LastTradesComponent,
    OrderbookComponent,
    TradingPanelComponent
  ],
  imports: [
    BrowserModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { ChartComponent } from './chart/chart.component';
import { TickersComponent } from './tickers/tickers.component';
import { NavbarComponent } from './navbar/navbar.component';
import { LastTradesComponent } from './last-trades/last-trades.component';
import { OrderbookComponent } from './orderbook/orderbook.component';
import { TradingPanelComponent } from './trading-panel/trading-panel.component';
import { PrimeSocketService } from './services/prime-socket.service';
import { LastTradesService } from './services/last-trades/last-trades.service';
import { OrderbookService } from './services/orderbook/orderbook.service';
import { TickerService } from './services/ticker/ticker.service';
import { HttpClientModule } from '@angular/common/http';
import { CandlesService } from './services/ohlc/candles.service';

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
    BrowserModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [
    PrimeSocketService,
    LastTradesService,
    OrderbookService,
    TickerService,
    CandlesService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

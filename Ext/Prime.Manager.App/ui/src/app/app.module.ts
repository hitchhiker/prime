import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { NgModule } from '@angular/core';
import { MaterialModule } from "./material.module";

import { AppComponent } from './app.component';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { LoggerService } from './services/logger.service';
import { WsClientService } from './services/ws-client.service';
import { PrimeSocketService } from './services/prime-socket.service';
import { FilterPipe } from './pipes/filter.pipe';
import { ChartService } from './services/chart.service';

@NgModule({
  declarations: [
    AppComponent,
    ToolbarComponent,
    FilterPipe,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
  ],
  providers: [
    LoggerService,
    {provide: "ISocketClient", useClass: WsClientService},
    PrimeSocketService,
    ChartService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

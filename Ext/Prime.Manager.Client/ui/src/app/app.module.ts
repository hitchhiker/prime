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
import { ChartService } from './services/chart.service';
import { ExtListComponent } from './extensions/ext-list/ext-list.component';
import { ExtComponent } from './extensions/ext/ext.component';
import { SettingsComponent } from './settings/settings.component';

@NgModule({
  declarations: [
    AppComponent,
    ToolbarComponent,
    ExtListComponent,
    ExtComponent,
    SettingsComponent,
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

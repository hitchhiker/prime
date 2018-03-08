import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { NgxElectronModule } from 'ngx-electron';

import { SocketIoModule, SocketIoConfig } from 'ng-socket-io';


import { AppComponent } from './app.component';
import { PrimeService } from './prime.service';
import { ProvidersListComponent } from './providers-list/providers-list.component';
import { ProvidersListItemComponent } from './providers-list-item/providers-list-item.component';

const config: SocketIoConfig = { url: "http://127.0.0.1:8082" }

@NgModule({
  declarations: [
    AppComponent,
    ProvidersListComponent,
    ProvidersListItemComponent
  ],
  imports: [
    BrowserModule,
    NgxElectronModule,
    SocketIoModule.forRoot(config)
  ],
  providers: [PrimeService],
  bootstrap: [AppComponent]
})
export class AppModule { }

import { Component, OnInit } from '@angular/core';
import { Exchange } from '../models/Exchange';
import { PrimeSocketService } from '../services/prime-socket.service';
import { PrivateProvidersListResponseMessage } from '../models/messages';
import { LoggerService } from '../services/logger.service';
import { MatSnackBar } from '@angular/material';
import { ActionThrottlerService } from '../services/action-throttler.service';

@Component({
  selector: 'app-exchanges',
  templateUrl: './exchanges.component.html',
  styleUrls: ['./exchanges.component.css']
})
export class ExchangesComponent implements OnInit {

  exchanges: Exchange[];
  exchangeFilter: string;

  constructor(
    private primeService: PrimeSocketService,
    private actionThrottler: ActionThrottlerService,
    public snackBar: MatSnackBar,
  ) {    
    primeService.onClientConnected.subscribe(() => {
      console.log("ExchangesComponent client connected.");
      this.snackBar.open("Connected to server", "Info", {
        duration: 3000
      });

      this.primeService.getPrivateProvidersList((data: PrivateProvidersListResponseMessage) => {
        this.exchanges = data.response;
      });
    });

    primeService.onErrorOccurred.subscribe(() => {
      this.snackBar.open("Connection error occurred", "Error", {
        duration: 3000
      });
    });
  }

  ngOnInit() {
    this.primeService.connect();
  }
}

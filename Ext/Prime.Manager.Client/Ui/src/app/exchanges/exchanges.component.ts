import { Component, OnInit } from '@angular/core';
import { Exchange } from '../models/Exchange';
import { PrimeSocketService } from '../services/prime-socket.service';
import { PrivateProvidersListResponseMessage } from '../models/messages';
import { LoggerService } from '../services/logger.service';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-exchanges',
  templateUrl: './exchanges.component.html',
  styleUrls: ['./exchanges.component.css']
})
export class ExchangesComponent implements OnInit {

  exchanges: Exchange[];

  constructor(
    private primeService: PrimeSocketService,
    public snackBar: MatSnackBar,
  ) { 
    primeService.onClientConnected = () => {
      this.snackBar.open("Connected to server", "Info", {
        duration: 3000
      });

      this.primeService.getPrivateProvidersList((data: PrivateProvidersListResponseMessage) => {
        LoggerService.logObj(data);
        this.exchanges = data.response;
      });
    }

    primeService.onErrorOccurred = () => {
      this.snackBar.open("Connection error occurred", "Error", {
        duration: 3000
      });
    };
  }

  ngOnInit() {
    this.primeService.connect();
  }
}

import { Component, OnInit, Input, Inject } from '@angular/core';
import { Exchange } from '../models/Exchange';

import {MatDialog, MatDialogConfig, MAT_DIALOG_DATA} from "@angular/material";
import { ExchangeDialogComponent } from '../exchange-dialog/exchange-dialog.component';
import { LoggerService } from '../services/logger.service';
import { PrimeSocketService } from '../services/prime-socket.service';

@Component({
  selector: 'app-exchange',
  templateUrl: './exchange.component.html',
  styleUrls: ['./exchange.component.css']
})
export class ExchangeComponent implements OnInit {
  constructor(
    private dialog: MatDialog,
    private primeSocket: PrimeSocketService
  ) { 

  }

  @Input() exchange: Exchange;

  openDialog(idHash: string) {
    LoggerService.log("Opening dialog (" + this.exchange.id + ")...");

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = true;
    dialogConfig.data = this.exchange;
    dialogConfig.width = "535px";

    let dialog = this.dialog.open(ExchangeDialogComponent, dialogConfig);
    dialog.beforeClose().subscribe(r => {
      this.primeSocket.checkProvidersKeys(idHash, (data) => {
        this.exchange.hasKeys = data.success;
      });
    });
  }

  ngOnInit() {
    this.primeSocket.onClientConnected = () => {
      console.log("Overried connection");
    }
  }

}


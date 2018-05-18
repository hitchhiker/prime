import { Component, OnInit, Input } from '@angular/core';
import { ExchangeDetails } from '../models/ExchangeDetails';
import { MatSnackBar } from '@angular/material';
import { PrimeSocketService } from '../services/prime-socket.service';
import { PrivateApiContext } from '../models/private-api-context';

@Component({
  selector: 'app-exchange-dialog',
  templateUrl: './exchange-dialog.component.html',
  styleUrls: ['./exchange-dialog.component.css']
})
export class ExchangeDialogComponent implements OnInit {

  extraEnabled: boolean = false;

  @Input() exchangeDetails: ExchangeDetails;
  constructor(
    public snackBar: MatSnackBar,
    private primeSocket: PrimeSocketService
  ) {
    this.exchangeDetails = new ExchangeDetails("Poloniex", 
      new PrivateApiContext("awdjashdk1j2h3", "Key", "Secret", "Extra"));
  }

  testPrivateApi() {
    if(!this.extraEnabled)
      this.exchangeDetails.privateApiContext.extra = null;

    this.primeSocket.testPrivateApi(
      this.exchangeDetails.privateApiContext
    );

    this.snackBar.open("Private API test!", "Info", {
      duration: 3000
    });
  }

  ngOnInit() {
  }

}

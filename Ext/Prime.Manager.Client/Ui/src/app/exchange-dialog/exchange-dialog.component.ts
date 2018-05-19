import { Component, OnInit, Input, Inject } from '@angular/core';
import { ExchangeDetails } from '../models/ExchangeDetails';
import { MatSnackBar, MAT_DIALOG_DATA } from '@angular/material';
import { PrimeSocketService } from '../services/prime-socket.service';
import { PrivateApiContext } from '../models/private-api-context';
import { LoggerService } from '../services/logger.service';
import { Exchange } from '../models/Exchange';
import { ProviderDetailsResponseMessage } from '../models/messages';

@Component({
  selector: 'app-exchange-dialog',
  templateUrl: './exchange-dialog.component.html',
  styleUrls: ['./exchange-dialog.component.css']
})
export class ExchangeDialogComponent implements OnInit {

  extraEnabled: boolean = false;

  @Input() exchangeDetails: ExchangeDetails = new ExchangeDetails("Provider", new PrivateApiContext("", "", ""));
  
  constructor(
    public snackBar: MatSnackBar,
    private primeSocket: PrimeSocketService,
    @Inject(MAT_DIALOG_DATA) public data: Exchange
  ) {
    //this.exchangeDetails = data;
    //LoggerService.logObj(data);
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

  saveApiKeys() {
    this.primeSocket.saveApiKeys();
  }

  ngOnInit() {
    this.primeSocket.getProviderDetails(this.data.id, (data: ProviderDetailsResponseMessage) => {
      this.exchangeDetails = new ExchangeDetails(
        data.response.name,
        new PrivateApiContext(
          data.response.id,
          data.response.key,
          data.response.secret,
          data.response.secret
        )
      );
    });
  }
}

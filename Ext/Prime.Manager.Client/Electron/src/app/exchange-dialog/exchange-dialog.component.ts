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

  private resetExchangeDetails() {
    if (this.exchangeDetails.privateApiContext === null) {
      this.exchangeDetails.privateApiContext = new PrivateApiContext("", "", "");
    }
    else {
      this.exchangeDetails.privateApiContext.key = "";
      this.exchangeDetails.privateApiContext.secret = "";
      this.exchangeDetails.privateApiContext.extra = "";
    }
  }

  testPrivateApi() {
    if (!this.extraEnabled)
      this.exchangeDetails.privateApiContext.extra = null;

    this.primeSocket.testPrivateApi(
      this.exchangeDetails.privateApiContext,
      (data) => {
        this.snackBar.open((data.success ? "API test succeeded" : (data.message != "" && data.message != null ? "API test error: " + data.message : "API test error occurred")), "Info", {
          duration: 3000
        });
      }
    );
  }

  extraChanged(event) {
    if (this.extraEnabled === false) {
      this.exchangeDetails.privateApiContext.extra = null;
    }
  }

  saveApiKeys() {
    if (this.extraEnabled === false) {
      this.exchangeDetails.privateApiContext.extra = null;
    }

    this.primeSocket.saveApiKeys(this.exchangeDetails, (data) => {
      this.snackBar.open((data.success ? "Keys saved" : "Error during saving: " + data.message), "Info", {
        duration: 3000
      });
    });
  }

  deleteKeys() {
    this.primeSocket.deleteKeys(this.exchangeDetails.id, (data) => {
      this.resetExchangeDetails();
      this.snackBar.open((data.success ? "Keys deleted" : "Error during saving: " + data.message), "Info", {
        duration: 3000
      });
    });
  }

  ngOnInit() {
    this.primeSocket.getProviderDetails(this.data.id, (data: ProviderDetailsResponseMessage) => {
      this.exchangeDetails = new ExchangeDetails(
        data.response.name,
        new PrivateApiContext(
          data.response.id,
          data.response.key,
          data.response.secret,
          data.response.extra
        )
      );

      if (data.response.extra !== null && data.response.extra !== "") {
        this.extraEnabled = true;
      }
    });
  }
}

import { Component, OnInit, ChangeDetectorRef, ApplicationRef } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { PrimeService } from './prime.service';
import { Observable } from 'rxjs/Observable';
import { ActionThrottleService } from './action-throttle.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  ngOnInit(): void {
    this.providersDisplay = this.providers;

    this._primeService.connect();

    this.loadPrivateProviders();
  }

  public providersDisplay: Array<any> = [];

  public providers: Array<any> = [
    { "Name": "Binance", "Id": "1k2j3hda" },
    { "Name": "Bitfinex", "Id": "awdjklk2jh" },
    { "Name": "Bittrex", "Id": "awdjklk2jh" },
    { "Name": "Bitkonan", "Id": "awdjklk2jh" },
  ];

  constructor(
    private _electronService: ElectronService, 
    private _primeService: PrimeService, 
    private _changeDetectorRef: ChangeDetectorRef,
    private _actionThrottleService: ActionThrottleService
  ) { }

  onFilterChange(searchValue: string) {
    this._actionThrottleService.throttle(300, () => {
      this.providersDisplay = this.providers.filter(x => x.Name.indexOf(searchValue) !== -1);
    });
  }

  launchWindow() {
    this._electronService.shell.openExternal('https://www.google.com/');
  }

  loadAllProviders() {
    this._primeService.getProvidersList((event, data) => {
      this.providers = JSON.parse(data);

      this._changeDetectorRef.detectChanges();
    });
  }

  loadPrivateProviders() {
    this._primeService.getPrivateProvidersList((event, data) => {
      this.providers = JSON.parse(data);
      this.providersDisplay = this.providers;
      console.log(this.providers);

      this._changeDetectorRef.detectChanges();
    });
  }

  loadPublicProviders() {

  }

  testPrime() {

  }

  title = 'app';
}

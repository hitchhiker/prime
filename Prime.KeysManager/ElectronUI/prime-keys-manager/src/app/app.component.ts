import { Component, OnInit } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { PrimeService } from './prime.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  ngOnInit(): void {
    this._primeService.connect();

    this._primeService.getProvidersList((data) => {
      console.log(data);
    })
  }

  public providers: Array<any> = [
    { Name: "Binance", Id: "123ad21" },
    { Name: "Bittrex", Id: "adwjkh1k2jh" }
  ];

  constructor(private _electronService: ElectronService, private _primeService: PrimeService) {}

  launchWindow() {
    this._electronService.shell.openExternal('https://www.google.com/');
  }

  testPrime() {

  }

  title = 'app';
}

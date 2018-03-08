import { Component, OnInit, ChangeDetectorRef, ApplicationRef } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { PrimeService } from './prime.service';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  ngOnInit(): void {
    this._primeService.connect();

    this._primeService.getProvidersList((event, data) => {
      this.providers = JSON.parse(data);

      this._changeDetectorRef.detectChanges();
    });
  }

  public providers: Array<any> = [];

  constructor(private _electronService: ElectronService, private _primeService: PrimeService, private _changeDetectorRef: ChangeDetectorRef) {}

  launchWindow() {
    this._electronService.shell.openExternal('https://www.google.com/');
  }

  testPrime() {

  }

  title = 'app';
}

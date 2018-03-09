import { Injectable, OnInit } from '@angular/core';
import 'rxjs/add/operator/map';
import { ElectronService } from 'ngx-electron';

@Injectable()
export class PrimeService implements OnInit {

  constructor(private _electronService: ElectronService) { }

  connect() {
    this._electronService.ipcRenderer.send('socket:connect', "data");
  }

  getProvidersList(callback: (event, data) => any) {
    this._electronService.ipcRenderer.send('socket:get-providers-list');

    this._electronService.ipcRenderer.on('socket:providers-list', callback);
  }

  getPrivateProvidersList(callback: (event, data) => any) {
    this._electronService.ipcRenderer.send('socket:get-private-providers-list');

    this._electronService.ipcRenderer.on('socket:private-providers-list', callback);
  }

  ngOnInit(): void {
    
  }

}

import { Injectable, OnInit } from '@angular/core';
import 'rxjs/add/operator/map';
import { ElectronService } from 'ngx-electron';

@Injectable()
export class PrimeService implements OnInit {

  constructor(private _electronService: ElectronService) { }

  connect() {
    this._electronService.ipcRenderer.send('socket:connect', "data");
  }

  getProvidersList(callback: (d) => any) {
    this._electronService.ipcRenderer.send('socket:get-providers-list');

    this._electronService.ipcRenderer.on('socket:providers-list', (event, data) => {
      console.log("Providers list listener handles call!");
      console.log(data);
    });
  }

  ngOnInit(): void {
    
  }

}

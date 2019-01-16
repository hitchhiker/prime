import { Injectable } from '@angular/core';

import { AppState } from 'src/app/models/app-state';
import { AssetPair } from '../models/trading/asset-pair';

@Injectable({
  providedIn: 'root'
})
export class AppService {

  private static _state: AppState = new AppState();

  get state(): AppState {
    return AppService._state;
  }

  constructor() { }
}

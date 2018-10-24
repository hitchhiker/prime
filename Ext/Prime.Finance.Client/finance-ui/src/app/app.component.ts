import { Component, OnInit } from '@angular/core';
import { PrimeSocketService } from './services/prime-socket.service';
import { AppService } from './services/app.service';
import { AssetPair } from './models/trading/asset-pair';
import { Asset } from './models/trading/asset';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  ngOnInit(): void {
    //this.primeClient.connect();
    
    this.app.state.setMarket(AssetPair.fromAssetCodes("BTC", "USD"));
    this.app.state.setLatestPrice(6312.4232);
  }

  title = 'app';

  constructor(
    private app: AppService,
    private primeClient: PrimeSocketService) {
  }

}

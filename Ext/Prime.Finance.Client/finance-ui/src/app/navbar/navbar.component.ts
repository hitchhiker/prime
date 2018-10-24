import { Component, OnInit } from '@angular/core';
import { AppService } from '../services/app.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  get ticker(): string {
    return this.app.state.market.tickerUi;
  }

  get latestPrice(): string {
    return this.app.state.latestPrice.toFixed(4);
  }

  constructor(
    private app: AppService
  ) { }

  ngOnInit() {
  }

}

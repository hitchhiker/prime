import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-trading-panel',
  templateUrl: './trading-panel.component.html',
  styleUrls: ['./trading-panel.component.scss']
})
export class TradingPanelComponent implements OnInit {

  constructor() { }

  selectedTabLimit: boolean = true;
  selectedTabMarket: boolean = false;

  selectTabMarket(): boolean {
    this.deselectAllTabs();

    this.selectedTabMarket = true;

    return false;
  }

  selectTabLimit(): boolean {
    this.deselectAllTabs();

    this.selectedTabLimit = true;

    return false;
  }

  private deselectAllTabs() {
    this.selectedTabLimit = false;
    this.selectedTabMarket = false;
  }

  ngOnInit() {
  }

}

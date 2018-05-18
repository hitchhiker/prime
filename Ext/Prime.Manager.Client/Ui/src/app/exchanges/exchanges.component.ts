import { Component, OnInit } from '@angular/core';
import { Exchange } from '../models/Exchange';

@Component({
  selector: 'app-exchanges',
  templateUrl: './exchanges.component.html',
  styleUrls: ['./exchanges.component.css']
})
export class ExchangesComponent implements OnInit {

  exchanges: Exchange[] = [
    new Exchange("Poloniex", "123awd23"),
    new Exchange("Bittrex", "123awd23"),
    new Exchange("Binance", "123awd23")
  ];

  constructor() { }

  ngOnInit() {
  }

}

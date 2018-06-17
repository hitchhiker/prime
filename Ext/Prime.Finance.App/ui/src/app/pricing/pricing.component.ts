import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { Market } from '../models/market';
import { PrimeSocketService } from '../services/prime-socket.service';
import { MatTableDataSource } from '@angular/material';
import { LatestPriceView } from '../models/latest-price';

@Component({
  selector: 'app-pricing',
  templateUrl: './pricing.component.html',
  styleUrls: ['./pricing.component.css']
})
export class PricingComponent implements OnInit {

  constructor(
    private prime: PrimeSocketService
  ) { }

  marketControl: FormControl = new FormControl();
  filterControl: FormControl = new FormControl();

  options: string[] = [];
  filteredOptions: Observable<string[]>;

  latestPrices: LatestPriceView[] = [
    { position: 1, exchangeName: "Binance", latestPrice: 1423 }
  ];

  displayedColumns = ['position', 'exchangeName', 'latestPrice'];
  dataSource = new MatTableDataSource(this.latestPrices);

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  ngOnInit() {
    this.prime.onClientConnected.subscribe(() => {
      console.log("PricingComponent client connected.");

      this.prime.getSupportedMarkets((data) => {
        console.log("Markets received.");
        console.log(data.markets.map((v) => v.code));

        for(let i = 0; i < data.markets.length; i++) {
          this.options.push(data.markets[i].code);
        }

        this.filteredOptions = this.marketControl.valueChanges.pipe(
          startWith(''),
          map(val => this.filter(val))
        );
      });
    });
  }

  filter(val: string): string[] {
    return this.options.filter(option => option.toLowerCase().indexOf(val.toLowerCase()) === 0);
  }

  marketSelected($event) {
    let marketName = $event.option.value;
    console.log("Selected market: " + marketName);
  }

}

import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { Market } from '../models/market';
import { PrimeSocketService } from '../services/prime-socket.service';

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

  ngOnInit() {
    this.filteredOptions = this.marketControl.valueChanges.pipe(
      startWith(''),
      map(val => this.filter(val))
    );
    this.prime.getSupportedMarkets((data) => {
      this.options = data.markets.map((v) => v.code);
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

import { Injectable, Output, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ChartService {

  constructor() { }

  isLoaded: boolean = false;

  load: EventEmitter<boolean> = new EventEmitter();

  loadChart() {
    if(this.isLoaded === false) {
      this.load.emit(this.isLoaded);
      this.isLoaded = true;
    }
  }
}

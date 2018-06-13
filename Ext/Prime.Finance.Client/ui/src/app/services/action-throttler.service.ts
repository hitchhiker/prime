import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ActionThrottlerService {
  private timer = null;

  throttle(period: number, action: () => void) {
    clearTimeout(this.timer);
    this.timer = setTimeout(function () {
      action();
    }, period);
  }
}

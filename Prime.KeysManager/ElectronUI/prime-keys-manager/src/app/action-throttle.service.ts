import { Injectable } from '@angular/core';

@Injectable()
export class ActionThrottleService {
  private timer: any = null;

  constructor() { }

  throttle(period: Number, action: () => any) {
    clearTimeout(this.timer);
    this.timer = setTimeout(() => {
      action();
    }, period);
  }

}

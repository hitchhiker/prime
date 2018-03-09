import { TestBed, inject } from '@angular/core/testing';

import { ActionThrottleService } from './action-throttle.service';

describe('ActionThrottleService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ActionThrottleService]
    });
  });

  it('should be created', inject([ActionThrottleService], (service: ActionThrottleService) => {
    expect(service).toBeTruthy();
  }));
});

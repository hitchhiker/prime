import { TestBed, inject } from '@angular/core/testing';

import { ActionThrottlerService } from './action-throttler.service';

describe('ActionThrottlerService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ActionThrottlerService]
    });
  });

  it('should be created', inject([ActionThrottlerService], (service: ActionThrottlerService) => {
    expect(service).toBeTruthy();
  }));
});

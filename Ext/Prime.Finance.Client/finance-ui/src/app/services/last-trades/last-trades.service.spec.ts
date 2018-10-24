import { TestBed, inject } from '@angular/core/testing';

import { LastTradesService } from '../last-trades.service';

describe('LastTradesService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LastTradesService]
    });
  });

  it('should be created', inject([LastTradesService], (service: LastTradesService) => {
    expect(service).toBeTruthy();
  }));
});

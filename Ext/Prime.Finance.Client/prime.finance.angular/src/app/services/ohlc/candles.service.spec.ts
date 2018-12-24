import { TestBed, inject } from '@angular/core/testing';

import { CandlesService } from './candles.service';

describe('CandlesService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CandlesService]
    });
  });

  it('should be created', inject([CandlesService], (service: CandlesService) => {
    expect(service).toBeTruthy();
  }));
});

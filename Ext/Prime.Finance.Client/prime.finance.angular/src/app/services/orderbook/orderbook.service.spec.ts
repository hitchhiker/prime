import { TestBed, inject } from '@angular/core/testing';

import { OrderbookService } from './orderbook.service';

describe('OrderbookService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [OrderbookService]
    });
  });

  it('should be created', inject([OrderbookService], (service: OrderbookService) => {
    expect(service).toBeTruthy();
  }));
});

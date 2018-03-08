import { TestBed, inject } from '@angular/core/testing';

import { PrimeService } from './prime.service';

describe('PrimeService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PrimeService]
    });
  });

  it('should be created', inject([PrimeService], (service: PrimeService) => {
    expect(service).toBeTruthy();
  }));
});

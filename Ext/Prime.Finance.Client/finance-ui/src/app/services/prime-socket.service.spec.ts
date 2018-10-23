import { TestBed, inject } from '@angular/core/testing';

import { PrimeSocketService } from './prime-socket.service';

describe('PrimeSocketService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PrimeSocketService]
    });
  });

  it('should be created', inject([PrimeSocketService], (service: PrimeSocketService) => {
    expect(service).toBeTruthy();
  }));
});

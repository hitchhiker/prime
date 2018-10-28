import { TestBed, inject } from '@angular/core/testing';

import { WsDataService } from './ws-data.service';

describe('WsDataService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [WsDataService]
    });
  });

  it('should be created', inject([WsDataService], (service: WsDataService) => {
    expect(service).toBeTruthy();
  }));
});

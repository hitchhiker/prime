import { TestBed, inject } from '@angular/core/testing';

import { WsBitfinexDataServiceBase } from './ws-bitfinex-data-base.service';

describe('WsDataService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [WsBitfinexDataServiceBase]
    });
  });

  it('should be created', inject([WsBitfinexDataServiceBase], (service: WsBitfinexDataServiceBase) => {
    expect(service).toBeTruthy();
  }));
});

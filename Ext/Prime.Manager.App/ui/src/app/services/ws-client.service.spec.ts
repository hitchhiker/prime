import { TestBed, inject } from '@angular/core/testing';

import { WsClientService } from './ws-client.service';

describe('WsClientService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [WsClientService]
    });
  });

  it('should be created', inject([WsClientService], (service: WsClientService) => {
    expect(service).toBeTruthy();
  }));
});

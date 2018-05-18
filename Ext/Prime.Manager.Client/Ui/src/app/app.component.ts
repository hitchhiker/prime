import { Component, OnInit } from '@angular/core';
import { WsClientService } from './services/ws-client.service';
import { ISocketClient } from './models/interfaces/ISocketClient';
import { PrimeSocketService } from './services/prime-socket.service';
import { LoggerService } from './services/logger.service';
import { ProvidersListResponseMessage } from './models/messages';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  ngOnInit(): void {
    this.primeTcpClient.connect(() => {

      this.primeTcpClient.getProviderProvidersList((data: ProvidersListResponseMessage) => {
        LoggerService.logObj(data);
      });
    });
  }
  
  private url = 'http://localhost:3001';
  private socket;

  constructor(private primeTcpClient: PrimeSocketService) {
    
  }

  title = 'Prime.KeysManager';
}

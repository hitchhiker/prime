import { Component, OnInit } from '@angular/core';
import { WsClientService } from './services/ws-client.service';
import { ISocketClient } from './models/interfaces/ISocketClient';
import { PrimeSocketService } from './services/prime-socket.service';

declare var loadSvg: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  

  ngOnInit(): void {
    setTimeout(function() {
      loadSvg()
    }, 1000);
  }
  
  private url = 'http://localhost:3001';
  private socket;

  constructor(private primeTcpClient: PrimeSocketService) {
    
  }

  title = 'Prime.KeysManager';
}

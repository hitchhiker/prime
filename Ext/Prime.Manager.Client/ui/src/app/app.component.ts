import { Component, OnInit } from '@angular/core';
import { WsClientService } from './services/ws-client.service';
import { ISocketClient } from './models/interfaces/ISocketClient';
import { PrimeSocketService } from './services/prime-socket.service';
import { ChartService } from './services/chart.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(
    private primeService: PrimeSocketService
  ) {

  }

  ngOnInit(): void {
    this.primeService.connect();
  }
}

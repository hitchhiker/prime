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

  ngOnInit(): void {

  }
  
  private url = 'http://localhost:3001';
  private socket;

  public selectedIndex;

  constructor(
    private primeTcpClient: PrimeSocketService,
    private chartService: ChartService
  ) {
    
  }

  selectionChanged() {
    const CHART_TAB_INDEX = 1;
    if(this.selectedIndex === CHART_TAB_INDEX) {
      this.chartService.loadChart();
    }
  }

  title = 'Prime.KeysManager';
}

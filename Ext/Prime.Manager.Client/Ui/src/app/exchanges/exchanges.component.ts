import { Component, OnInit } from '@angular/core';
import { Exchange } from '../models/Exchange';
import { PrimeSocketService } from '../services/prime-socket.service';
import { ProvidersListResponseMessage } from '../models/messages';
import { LoggerService } from '../services/logger.service';

@Component({
  selector: 'app-exchanges',
  templateUrl: './exchanges.component.html',
  styleUrls: ['./exchanges.component.css']
})
export class ExchangesComponent implements OnInit {

  exchanges: Exchange[];

  constructor(private primeTcpClient: PrimeSocketService) { }

  ngOnInit() {
    this.primeTcpClient.connect(() => {

      this.primeTcpClient.getProviderProvidersList((data: ProvidersListResponseMessage) => {
        LoggerService.logObj(data);
        this.exchanges = data.response;
      });
    });
  }

}

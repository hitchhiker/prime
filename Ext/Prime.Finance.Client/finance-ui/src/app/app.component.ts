import { Component, OnInit } from '@angular/core';
import { PrimeSocketService } from './services/prime-socket.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  ngOnInit(): void {
    this.primeClient.connect();
  }

  title = 'app';

  constructor(private primeClient: PrimeSocketService) {
    
  }

}

import { Component, OnInit } from '@angular/core';
import { PrimeSocketService } from '../services/prime-socket.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit {

  constructor(
    private primeService: PrimeSocketService
  ) { }

  isUtsTime: boolean = true;
  isUtcChanged() {
    this.primeService.updateTimeKind(this.isUtsTime, (data) => {
      console.log("time updated, reponse:");
      console.log(data);
    });
  }

  ngOnInit() {
  }

}

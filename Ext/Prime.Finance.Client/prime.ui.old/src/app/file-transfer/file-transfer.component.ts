import { Component, OnInit } from '@angular/core';
import { PrimeSocketService } from '../services/prime-socket.service';

@Component({
  selector: 'app-file-transfer',
  templateUrl: './file-transfer.component.html',
  styleUrls: ['./file-transfer.component.css']
})
export class FileTransferComponent implements OnInit {

  constructor(
    private primeService: PrimeSocketService
  ) { }

  ngOnInit() {
  }

  downloadFile() {
    console.log("Loading data...");
    this.primeService.downloadFile((data) => {
      var file = data.fileBase64;
      console.log(file);
    });
  }

}

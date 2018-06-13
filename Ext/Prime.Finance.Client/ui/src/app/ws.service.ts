import { Injectable } from '@angular/core';
import { LoggerService } from './services/logger.service';

import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class WsService {

  constructor() { }

  run() {
    LoggerService.log("Ws service running...");

    //let ws = new WebSocket("ws://echo.websocket.org/");
    let ws = new WebSocket("ws://0.0.0.0:8081");
    ws.onopen = () => {
      //LoggerService.log("Connected... Sending");
      ws.send("Hello!");
    };
    ws.onmessage = (e) => {
      LoggerService.log("data: " + JSON.stringify(e.data));
    };
   

    //let socket = io("");
  }
}

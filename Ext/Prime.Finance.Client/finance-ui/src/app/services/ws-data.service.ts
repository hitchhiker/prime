import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { WebsocketService } from './websocket.service';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class WsDataService {

  private wsSubject: Subject<any>;

  private _wsOnOpen: Subject<MessageEvent> = new Subject();
  private _wsOnMessage: Subject<MessageEvent> = new Subject();

  protected wsOnConnected: Observable<MessageEvent> = this._wsOnOpen.asObservable();
  protected wsOnMessage: Observable<MessageEvent> = this._wsOnMessage.asObservable();

  private _endpointURL: string;

  protected setEndpointUrl(url: string) {
    this._endpointURL = url;

    console.log(this._endpointURL);
  }

  constructor(
    private ws: WebsocketService
  ) { }

  protected subscribeToMessages(action: (msg: MessageEvent) => void) {
    if (!this.wsSubject)
      Error("WS client should be connected first.");

    this.wsSubject.subscribe(action);
  }

  protected sendMessage(msg: any) {
    this.wsSubject.next(msg);
  }

  protected connectToEndpoint() {
    if (!this._endpointURL)
      Error("WS endpoint is not set.");
      
    this.wsSubject = <Subject<any>>this.ws.connect(this._endpointURL)
      .pipe(map((msg): MessageEvent => {
        return msg;
      }));

    // Generic messages handling.
    this.wsSubject.subscribe((msg: MessageEvent) => {
      if (msg.type === "open") {
        this._wsOnOpen.next(msg);
      } else if (msg.type === "message") {
        this._wsOnMessage.next(msg);
      }
    });
  }
}


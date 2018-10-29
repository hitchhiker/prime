import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { WebsocketService } from './websocket.service';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export abstract class WsDataService {

  private wsSubject: Subject<any>;

  private _wsOnOpen: Subject<MessageEvent> = new Subject();
  private _wsOnMessage: Subject<MessageEvent> = new Subject();

  protected wsOnConnected: Observable<MessageEvent> = this._wsOnOpen.asObservable();
  protected wsOnMessage: Observable<MessageEvent> = this._wsOnMessage.asObservable();

  protected abstract get endpointURL(): string;

  constructor(
    private ws: WebsocketService
  ) { }

  public abstract connect();

  protected subscribeToMessages(action: (msg: MessageEvent) => void) {
    this.wsSubject.subscribe(action);
  }

  protected sendMessage(msg: any) {
    this.wsSubject.next(msg);
  }

  protected connectToEndpoint() {     
    this.wsSubject = <Subject<any>>this.ws.connect(this.endpointURL)
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


import { Subject, Observable } from 'rxjs';
import { WebsocketService } from './websocket.service';
import { map } from 'rxjs/operators';

export abstract class WsBitfinexDataServiceBase {
  private wsSubject: Subject<any>;
  private _isConnected: boolean = false;

  private _endpointURL: string = "wss://api.bitfinex.com/ws/2";

  constructor(
    private ws: WebsocketService
  ) { }

  public abstract onConnectedHandler(msgEvent: MessageEvent);
  public abstract onMessageHandler(msgEvent: MessageEvent);

  protected sendMessage(msg: any) {
    if(this._isConnected === false) {
      throw Error("WS is not connected to endpoint. Please call 'connectToEndpoint' method first.");
    }

    this.wsSubject.next(msg);
  }

  public connect() {     
    this.wsSubject = <Subject<any>>this.ws.connect(this._endpointURL)
      .pipe(map((msg): MessageEvent => {
        return msg;
      }));

    // Generic messages handling.
    this.wsSubject.subscribe((msgEvent: MessageEvent) => {
      if (msgEvent.type === "open") {
        this._isConnected = true;
        this.onConnectedHandler(msgEvent);
      } else if (msgEvent.type === "message") {
        this.onMessageHandler(msgEvent);
      }
    });
  }
}


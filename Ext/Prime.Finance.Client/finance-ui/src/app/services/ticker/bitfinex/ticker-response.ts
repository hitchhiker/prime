import { ResponseBase } from "src/app/models/response-base";
import { RemoteResponse } from "src/app/models/remote-response";
import { ResponseType } from "../../last-trades/bitfinex/response-type";
import { Ticker } from "./ticker";

export class TickerResponse implements ResponseBase {

    constructor(private rRaw: any[]) { }

    parseResponse(): RemoteResponse {
        if(typeof this.rRaw[1] === "object" && this.rRaw[1].length === 10) {
            // All ok, that's ticker response.
            let rArray = this.rRaw[1];
            let response: Ticker = {
                highestBid: rArray[0],
                highestAsk: rArray[2],
                dailyChange: rArray[4],
                lastPrice: rArray[6],
                volumeDaily: rArray[7]
            };
            
            return {
                responseType: ResponseType.TransactionUpdated,
                payload: response
            };
        }

        return null;
    }
}

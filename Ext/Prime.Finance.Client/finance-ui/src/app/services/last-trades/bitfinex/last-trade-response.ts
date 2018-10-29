import { RemoteResponse } from "../../../models/remote-response";
import { LastTrade } from "src/models/view/last-trade";
import { DateTime } from "../date-time";
import { OrderSide } from "src/app/models/trading/order-side";
import { ResponseType } from "./response-type";
import { ResponseBase } from "src/app/models/response-base";

// Maps array response to object.
export class LastTradeRespose implements ResponseBase {
  public constructor(
    private rRaw: any[]
    ) { }

  private getLastTrade(r: any[]): LastTrade {
    let lastTradeR: LastTradeRespose = new LastTradeRespose(r);

    return {
      Id: r[0],
      Amount: Math.abs(r[2]),
      Date: DateTime.fromUnixTimestamp(r[1]),
      Price: r[3],
      Type: r[2] >= 0 ? OrderSide.Buy : OrderSide.Sell
    };
  }

  public toRemoteResponse(): RemoteResponse {
    if (this.rRaw.length === 2) {
      // Snapshot or heartbeat.
      if (typeof this.rRaw[1] == "string") {
        // Heartbeat.
      } else {
        // Snapshot.

        this.rRaw = this.rRaw.sort((a, b) => {
          return a[1] >= b[1] ? 1 : 0;
        });

        // Get LastTrade objects array.
        let snapshotTrades: Array<LastTrade> = [];

        for (let i = 0; i < this.rRaw[1].length; i++) {
          snapshotTrades.push(this.getLastTrade(this.rRaw[1][i]));
        }

        snapshotTrades

        let result: RemoteResponse = {
          responseType: ResponseType.Snapshot,
          payload: snapshotTrades.sort((a, b) => {
            return a.Date.date >= b.Date.date ? 1 : 0
          })
        };

        return result;
      }
    }
    else if (this.rRaw.length === 3) {
      // Updated.
      if (this.rRaw[1] === "tu") {
        // Notify.
        let result: RemoteResponse = {
          responseType: ResponseType.TransactionUpdated,
          payload: this.getLastTrade(this.rRaw[2])
        };

        return result;
      }
    }
  }
}

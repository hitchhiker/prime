import { TradeInfo } from "./trade-info";
import { LastTrade } from "src/models/view/last-trade";
import { DateTime } from "../date-time";
import { OrderSide } from "src/app/models/trading/order-side";
import { ResponseType } from "./response-type";

// Maps array response to object.
export class LastTradeRespose {
  ID: number;
  MTS: number;
  AMOUNT: number;
  PRICE: number;

  private constructor() { }

  private static getLastTradeFromArray(r: any[]): LastTrade {
    let lastTradeR = LastTradeRespose.fromArray(r);

    return {
      Id: lastTradeR.ID,
      Amount: Math.abs(lastTradeR.AMOUNT),
      Date: DateTime.fromUnixTimestamp(lastTradeR.MTS),
      Price: lastTradeR.PRICE,
      Type: lastTradeR.AMOUNT >= 0 ? OrderSide.Buy : OrderSide.Sell
    };
  }

  public static fromRawResponse(rRaw: any): TradeInfo {
    if (rRaw.length === 2) {
      // Snapshot or heartbeat.
      if (typeof rRaw[1] == "string") {
        // Heartbeat.
      } else {
        // Snapshot.

        rRaw = rRaw.sort((a, b) => {
          return a[1] >= b[1] ? 1 : 0;
        });

        // Get LastTrade objects array.
        let snapshotTrades: Array<LastTrade> = [];

        for (let i = 0; i < rRaw[1].length; i++) {
          snapshotTrades.push(LastTradeRespose.getLastTradeFromArray(rRaw[1][i]));
        }

        snapshotTrades

        let result: TradeInfo = {
          responseType: ResponseType.Snapshot,
          payload: snapshotTrades.sort((a, b) => {
            return a.Date.date >= b.Date.date ? 1 : 0
          })
        };

        return result;
      }
    }
    else if (rRaw.length === 3) {
      // Updated.
      if (rRaw[1] === "tu") {
        // Notify.
        let result: TradeInfo = {
          responseType: ResponseType.TransactionUpdated,
          payload: LastTradeRespose.getLastTradeFromArray(rRaw[2])
        };

        return result;
      }
    }
  }

  public static fromArray(r: any[]): LastTradeRespose {
    return {
      ID: r[0],
      MTS: r[1],
      AMOUNT: r[2],
      PRICE: r[3],
    };
  }
}

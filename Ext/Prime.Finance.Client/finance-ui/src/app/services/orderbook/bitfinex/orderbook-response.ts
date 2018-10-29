import { OrderbookOrder } from "src/models/view/orderbook-order";
import { OrderSide } from "src/app/models/trading/order-side";
import { ResponseBase } from "src/app/models/response-base";
import { RemoteResponse } from "src/app/models/remote-response";
import { ResponseType } from "../../last-trades/bitfinex/response-type";

export class OrderbookResponse implements ResponseBase {
    public toRemoteResponse(): RemoteResponse {
        
        if (this.rRaw) {
            console.log(this.rRaw);
            if (this.rRaw[1].length > 3 && typeof this.rRaw[1][0] === "object") {
                // Snapshot.

                let orderbook: OrderbookOrder[] = [];
                for (let i = 0; i < this.rRaw[1].length; i++) {
                    let cItem = this.rRaw[1][i];
                    let orderbookRecord: OrderbookOrder = this.getOrderbookRecord(cItem);
                    orderbook.push(orderbookRecord);
                }

                return {
                    responseType: ResponseType.Snapshot,
                    payload: orderbook
                }
            }
            else if (this.rRaw[1].length === 3) {
                // Updated.
                return {
                    responseType: ResponseType.TransactionUpdated,
                    payload: this.getOrderbookRecord(this.rRaw[1])
                }
            }
        }

        return null;
    }

    private getOrderbookRecord(array: any[]): OrderbookOrder {
        let order = new OrderbookOrder();
        order.Id = array[0];
        order.Amount = Math.abs(array[2]);
        order.Price = Math.abs(array[1]);
        order.Side = array[2] >= 0 ? OrderSide.Buy : OrderSide.Sell;
        order.Sum = -1;

        return order;
    }

    constructor(
        private rRaw: any[]
    ) {

    }

}

import { OrderSide } from "./order-side";
import { AssetPair } from "./asset-pair";
import { OrderType } from "./order-type";

export class OrderContext {
    side: OrderSide;
    type: OrderType;
    amount: number;
    price: number;
    market: AssetPair;
}

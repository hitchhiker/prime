import { OrderSide } from "src/app/models/trading/order-side";

export class OrderbookOrder {
    Id: number;
    Sum: number;
    get Value(): number {
        return this.Amount * this.Price;
    }
    Amount: number;
    Price: number;
    Side: OrderSide;
}

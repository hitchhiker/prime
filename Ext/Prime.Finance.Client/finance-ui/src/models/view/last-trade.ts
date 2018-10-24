import { OrderSide } from "src/app/models/trading/order-side";

export class LastTrade {
    Id: number;
    Date: string;
    Type: OrderSide;
    Price: number;
    Amount: number;
}

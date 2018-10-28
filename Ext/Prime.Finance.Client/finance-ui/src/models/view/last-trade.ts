import { OrderSide } from "src/app/models/trading/order-side";
import { DateTime } from "src/app/services/last-trades/date-time";

export class LastTrade {
    Id: number;
    Date: DateTime;
    Type: OrderSide;
    Price: number;
    Amount: number;
}

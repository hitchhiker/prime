import { Exchange } from "./exchange";
import { PrivateApiContext } from "./private-api-context";

export class ExchangeDetails extends Exchange {
    constructor(
        title: string,
        public privateApiContext: PrivateApiContext
    ) {
        super(title, privateApiContext.exchangeId);
    }
}

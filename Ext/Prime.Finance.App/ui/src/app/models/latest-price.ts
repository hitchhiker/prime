
export class LatestPrice {
    exchangeName: string;
    latestPrice: number;
}

export class LatestPriceView extends LatestPrice {
    constructor(latestPriceModel: LatestPrice) {
        super();       

        this.exchangeName = latestPriceModel.exchangeName;
        this.latestPrice = latestPriceModel.latestPrice;
    }

    position: number;
}

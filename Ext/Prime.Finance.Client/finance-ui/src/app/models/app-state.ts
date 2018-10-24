import { AssetPair } from "./trading/asset-pair";

export class AppState {
    private _market: AssetPair;
    private _latestPrice: number;

    get market(): AssetPair {
        return this._market;
    }

    get latestPrice(): number {
        return this._latestPrice;
    }

    setMarket(newMarket: AssetPair): AssetPair {
        this._market = newMarket;
        return this._market;
    }

    setLatestPrice(newPrice: number) {
        this._latestPrice = newPrice;
    }
}

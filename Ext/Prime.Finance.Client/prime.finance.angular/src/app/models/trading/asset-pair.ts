import { Asset } from "./asset";

export class AssetPair {
    base: Asset;
    quote: Asset;

    static readonly tickerSeparator: string = "/";

    private constructor(baseAsset: Asset, quoteAsset: Asset) {
        this.base = baseAsset;
        this.quote = quoteAsset;
    }

    static fromAssets(baseAsset: Asset, quoteAsset: Asset) {
        return new AssetPair(baseAsset, quoteAsset);
    }

    static fromAssetCodes(baseAssetCode: string, quoteAssetCode: string) {
        return new AssetPair({ code: baseAssetCode }, { code: quoteAssetCode });
    }

    get ticker(): string {
        return `${this.base.code}${AssetPair.tickerSeparator}${this.quote.code}`;
    }

    get tickerUi(): string {
        return `${this.base.code} ${AssetPair.tickerSeparator} ${this.quote.code}`;
    }
}

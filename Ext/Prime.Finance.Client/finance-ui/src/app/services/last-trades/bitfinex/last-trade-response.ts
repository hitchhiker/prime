
export class LastTradeRespose {
    ID: number;
    MTS: number;
    AMOUNT: number;
    PRICE: number;

    private constructor() {}

    public static fromArray(r: any[]): LastTradeRespose {
        return {
            ID: r[0],
            MTS: r[1],
            AMOUNT: r[2],
            PRICE: r[3],
        };
    }
}

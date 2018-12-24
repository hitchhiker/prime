
export class PrivateApiContext {
    constructor(
        public exchangeId: string,
        public key: string,
        public secret: string,
        public extra?: string
    ) {  }
}

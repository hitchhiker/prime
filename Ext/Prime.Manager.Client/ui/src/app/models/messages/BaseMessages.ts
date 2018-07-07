
export abstract class BaseMessage {
    abstract $type: string;

    serialize(): string {
        return JSON.stringify(this, (key, value) => {
            if (value !== null && key !==  "expectedEmptyResponse") return value;
        });
    }
}

export abstract class BaseRequestMessage extends BaseMessage {
    constructor() {
        super();

        this.expectedEmptyResponse = this.createExpectedEmptyResponse();
    }

    abstract createExpectedEmptyResponse(): BaseMessage;
    expectedEmptyResponse: BaseMessage;
}

export abstract class BaseResponseMessage extends BaseMessage {
    sessionId: string;
}

export abstract class BooleanResponseMessage extends BaseResponseMessage {
    success: boolean;
    message: string;
}



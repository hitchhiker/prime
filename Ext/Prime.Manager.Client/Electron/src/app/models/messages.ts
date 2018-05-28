import { Exchange } from "./Exchange";

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

export class UserMessageRequest extends BaseMessage {
    $type: string = UserMessageRequest.name;

    UserName: string;
    Age: string;
}

export class ProvidersListRequestMessage extends BaseMessage {
    $type: string = "prime.manager.providerslistrequestmessage";
}
export class ProvidersListResponseMessage extends BaseResponseMessage {
    $type: string = "prime.manager.providerslistresponsemessage";

    response: Exchange[];
}

export class PrivateProvidersListRequestMessage extends BaseRequestMessage {
    createExpectedEmptyResponse(): BaseMessage {
        return new PrivateProvidersListResponseMessage();
    }

    $type: string = "prime.manager.privateproviderslistrequestmessage";
}
export class PrivateProvidersListResponseMessage extends BaseResponseMessage {
    $type: string = "prime.manager.privateproviderslistresponsemessage";

    response: Exchange[];
}

export class DeleteProviderKeysRequestMessage extends BaseRequestMessage {
    createExpectedEmptyResponse(): BaseMessage {
        return new DeleteProviderKeysResponseMessage();
    }

    $type: string = "prime.manager.deleteproviderkeysrequestmessage";
    id: string;
}
export class DeleteProviderKeysResponseMessage extends BooleanResponseMessage {
    $type: string = "prime.manager.deleteproviderkeysresponsemessage";
}

export class ProviderDetailsRequestMessage extends BaseRequestMessage {
    createExpectedEmptyResponse(): BaseMessage {
        return new ProviderDetailsResponseMessage();
    }

    $type: string = "prime.manager.providerdetailsrequestmessage";
    id: string;

    constructor(
        id: string
    ) {
        super();

        this.id = id;
    }
}
export class ProviderDetailsResponseMessage extends BaseResponseMessage {
    $type: string = "prime.manager.providerdetailsresponsemessage";

    response: {
        extra: string;
        id: string;
        key: string;
        name: string;
        secret: string;
    }
}

export class ProviderSaveKeysRequestMessage extends BaseRequestMessage {
    createExpectedEmptyResponse(): BaseMessage {
        return new ProviderSaveKeysResponseMessage();
    }

    $type: string = "prime.manager.providersavekeysrequestmessage";
    id: string;
    key: string;
    secret: string;
    extra: string
}
export class ProviderSaveKeysResponseMessage extends BooleanResponseMessage {
    $type: string = "prime.manager.providersavekeysresponsemessage";
}

export class TestPrivateApiRequestMessage extends BaseRequestMessage {
    createExpectedEmptyResponse(): BaseMessage {
        return new TestPrivateApiResponseMessage();
    }

    $type: string = "prime.manager.testprivateapirequestmessage";

    id: string;
    key: string;
    secret: string;
    extra: string;
}
export class TestPrivateApiResponseMessage extends BooleanResponseMessage {
    $type: string = "prime.manager.testprivateapiresponsemessage";
}

export class ProviderHasKeysRequestMessage extends BaseRequestMessage {
    createExpectedEmptyResponse(): BaseMessage {
        return new ProviderHasKeysResponseMessage();
    }

    $type: string = "prime.manager.providerhaskeysrequestmessage";
    id: string;
}
export class ProviderHasKeysResponseMessage extends BooleanResponseMessage {
    $type: string = "prime.manager.providerhaskeysresponsemessage";
}

export class ProviderKeysMessageRequest extends BaseMessage {
    $type: string = ProviderKeysMessageRequest.name;
}
export class ProviderKeysMessageResponse {
    Id: string;
    Key: string;
    Extra: string;
}



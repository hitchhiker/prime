
export abstract class BaseMessage {
    abstract $type: string;

    serialize(): string {
        return JSON.stringify(this, (key, value) => {
            if(value !== null) return value;
        });
    }
}

export abstract class BaseResponseMessage extends BaseMessage {
    sessionId: string;
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

    response: any[];
}

export class ProviderDetailsRequestMessage extends BaseMessage {
    $type: string = "prime.manager.providerdetailsrequestmessage";
    id: string;

    constructor(id: string) {
        super();

        this.id = id;
    }
}
export class ProviderDetailsResponseMessage extends BaseResponseMessage {
    $type: string = "prime.manager.providerdetailsresponsemessage";
}

export class TestPrivateApiMessageRequest extends BaseMessage {
    $type: string = TestPrivateApiMessageRequest.name;

    constructor(
        public ExchangeId: string,
        public Key: string,
        public Secret: string,
        public Extra?: string
    ) {
        super();
    }
}
export class TestPrivateApiMessageResponse {

}

export class ProviderKeysMessageRequest extends BaseMessage {
    $type: string = ProviderKeysMessageRequest.name;
}
export class ProviderKeysMessageResponse {
    Id: string;
    Key: string;
    Extra: string;
}



import { IpcMessageBase } from "./IpcMessageBase";

// Example of IpcMessageBase usage.
export class HelloMessage extends IpcMessageBase {
    protected channelBaseId(): string {
        return HelloMessage.name;
    }
}

export class GetPrivateProvidersListMessage extends IpcMessageBase {
    protected channelBaseId(): string {
        return GetPrivateProvidersListMessage.name;
    }
}

export class GetProviderDetailsMessage extends IpcMessageBase {
    protected channelBaseId(): string {
        return GetProviderDetailsMessage.name;
    }
}

export class SaveProviderKeysMessage extends IpcMessageBase {
    protected channelBaseId(): string {
        return SaveProviderKeysMessage.name;
    }
}

export class DeleteProviderKeysMessage extends IpcMessageBase {
    protected channelBaseId(): string {
        return DeleteProviderKeysMessage.name;
    }
}

export class TestPrivateApiMessage extends IpcMessageBase {
    protected channelBaseId(): string {
        return TestPrivateApiMessage.name;
    }
}

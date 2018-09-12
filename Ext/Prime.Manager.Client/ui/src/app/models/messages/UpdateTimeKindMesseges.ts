import { BaseRequestMessage, BaseMessage, BaseResponseMessage } from "./BaseMessages";

export class UpdateTimeKindRequestMessage extends BaseRequestMessage {
    $type: string = "prime.settings.timekinduiupdatedmessage";

    isUtc: boolean;

    createExpectedEmptyResponse(): BaseMessage {
        return new UpdateTimeKindResponseMessage();
    }
}

export class UpdateTimeKindResponseMessage extends BaseResponseMessage {
    $type: string = "prime.settings.updatetimekindresponsemessage";

    currentTime: string;
}

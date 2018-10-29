import { RemoteResponse } from "./remote-response";


export interface ResponseBase {
    toRemoteResponse(): RemoteResponse;
}

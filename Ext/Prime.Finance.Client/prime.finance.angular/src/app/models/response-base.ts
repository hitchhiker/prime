import { RemoteResponse } from "./remote-response";


export interface ResponseBase {
    parseResponse(): RemoteResponse;
}

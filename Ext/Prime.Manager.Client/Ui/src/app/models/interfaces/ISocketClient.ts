
export interface ISocketClient {
    connect(host: string); 
    write(data: string);
    
    onClientConnected: () => void;
    onDataReceived: (data: any) => void;
    onConnectionClosed: () => void;
}

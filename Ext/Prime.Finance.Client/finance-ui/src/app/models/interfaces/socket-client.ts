
export interface SocketClient {
    connect(host: string); 
    write(data: string);
    
    onClientConnected: () => void;
    onDataReceived: (data: any) => void;
    onConnectionClosed: () => void;
    onErrorOccurred: () => void;
}

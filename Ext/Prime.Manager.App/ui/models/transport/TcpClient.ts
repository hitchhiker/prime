import { Socket } from 'net';

export class TcpClient {
    private socketClient: Socket;

    onClientConnected: () => void;
    onDataReceived: (data: any) => void;
    onConnectionClosed: () => void;

    constructor() {
        this.socketClient = new Socket();
    }

    connect(port: number, host: string) {
        this.socketClient.on('data', this.onDataReceived);
        this.socketClient.on('close', this.onConnectionClosed);

        this.socketClient.connect(port, host, this.onClientConnected);
    }

    write(data: string) {
        this.socketClient.write(data);
    }
}

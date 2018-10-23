
module.exports = class FinServer {
    constructor(name) {
        this.name = name;
    }

    handleMessage(client, data) {
        console.log(data);
    }
}

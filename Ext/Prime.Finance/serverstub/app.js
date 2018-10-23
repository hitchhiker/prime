"use strict";
 
const wsServer = require("ws-server");
const http = require("http");

const FinServer = require("./fin.server");
let fs = new FinServer();
 
let httpServer = http.createServer();
let server = wsServer({ server: httpServer }, {
    timeout: 60000,
    
    connectionHandler: function(client) {
        console.log("Connection opened:", client.request.connection.remoteAddress);
    },
    
    messageHandler: function(data, client) {
        fs.handleMessage(client.request.connection.remoteAddress, data);
        //console.log(`Message from ${client.request.connection.remoteAddress}:`, data);
    },
    
    closeHandler: function(code, reason, client) {
        console.log("Connection closed:", client.request.connection.remoteAddress, code, reason);
    }
});
 
httpServer.listen(process.env.PORT || 9992);
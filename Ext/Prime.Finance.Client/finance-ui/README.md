# Finance UI

This project is a front-end for Prime.Finance app. It is based on Angular v6.0.0.
It can be built as an Electron app as well as launched on a development web server.

## Development web server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Electron app

1. Rename `electron-main.js` file to `main.js`. 
The `main.js` file is used to start Prime.Finance in the Electron container. In order to make Angular app debugging working there should be no file called `main.js` in the root folder. That's why before running and testing inside electron it has to be named differently, for example `electron-main.js`.
2. Run `npm run electron`.
This command builds Angular app and runs Electron container.
3. Rename `main.js` to `electron-main.js` to enable Angular web app debugging.

## Screenshot (demo)

![Prime.Finance demo inside Electron container](https://raw.githubusercontent.com/hitchhiker/prime/master/Ext/Prime.Finance.Client/finance-ui/sc-electron-1.png)

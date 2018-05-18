(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["main"],{

/***/ "./src/$$_lazy_route_resource lazy recursive":
/*!**********************************************************!*\
  !*** ./src/$$_lazy_route_resource lazy namespace object ***!
  \**********************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

function webpackEmptyAsyncContext(req) {
	// Here Promise.resolve().then() is used instead of new Promise() to prevent
	// uncaught exception popping up in devtools
	return Promise.resolve().then(function() {
		var e = new Error('Cannot find module "' + req + '".');
		e.code = 'MODULE_NOT_FOUND';
		throw e;
	});
}
webpackEmptyAsyncContext.keys = function() { return []; };
webpackEmptyAsyncContext.resolve = webpackEmptyAsyncContext;
module.exports = webpackEmptyAsyncContext;
webpackEmptyAsyncContext.id = "./src/$$_lazy_route_resource lazy recursive";

/***/ }),

/***/ "./src/app/app.component.css":
/*!***********************************!*\
  !*** ./src/app/app.component.css ***!
  \***********************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "\r\n.tab-content {\r\n    overflow-y: auto;\r\n}\r\n\r\n.tab-chart {\r\n    height: 500px;\r\n}\r\n\r\n.mat-tab-group {\r\n    padding-top: 64px;\r\n}\r\n\r\n\r\n"

/***/ }),

/***/ "./src/app/app.component.html":
/*!************************************!*\
  !*** ./src/app/app.component.html ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<!--The content below is only a placeholder and can be replaced.-->\n\n<app-toolbar></app-toolbar>\n\n<mat-tab-group>\n    <mat-tab label=\"Exchanges\">\n        <div class=\"tab-content tab-exchanges\">\n            <app-exchanges></app-exchanges>\n        </div>\n\n    </mat-tab>\n    <mat-tab label=\"Chart\">\n        <div class=\"tab-content tab-chart\">\n            Hello\n        </div>\n    </mat-tab>\n</mat-tab-group>"

/***/ }),

/***/ "./src/app/app.component.ts":
/*!**********************************!*\
  !*** ./src/app/app.component.ts ***!
  \**********************************/
/*! exports provided: AppComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppComponent", function() { return AppComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _services_prime_socket_service__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./services/prime-socket.service */ "./src/app/services/prime-socket.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var AppComponent = /** @class */ (function () {
    function AppComponent(primeTcpClient) {
        this.primeTcpClient = primeTcpClient;
        this.url = 'http://localhost:3001';
        this.title = 'Prime.KeysManager';
        primeTcpClient.connect();
    }
    AppComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-root',
            template: __webpack_require__(/*! ./app.component.html */ "./src/app/app.component.html"),
            styles: [__webpack_require__(/*! ./app.component.css */ "./src/app/app.component.css")]
        }),
        __metadata("design:paramtypes", [_services_prime_socket_service__WEBPACK_IMPORTED_MODULE_1__["PrimeSocketService"]])
    ], AppComponent);
    return AppComponent;
}());



/***/ }),

/***/ "./src/app/app.module.ts":
/*!*******************************!*\
  !*** ./src/app/app.module.ts ***!
  \*******************************/
/*! exports provided: AppModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppModule", function() { return AppModule; });
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/platform-browser */ "./node_modules/@angular/platform-browser/fesm5/platform-browser.js");
/* harmony import */ var _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/platform-browser/animations */ "./node_modules/@angular/platform-browser/fesm5/animations.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _material_module__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./material.module */ "./src/app/material.module.ts");
/* harmony import */ var _app_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./app.component */ "./src/app/app.component.ts");
/* harmony import */ var _exchanges_exchanges_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./exchanges/exchanges.component */ "./src/app/exchanges/exchanges.component.ts");
/* harmony import */ var _toolbar_toolbar_component__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./toolbar/toolbar.component */ "./src/app/toolbar/toolbar.component.ts");
/* harmony import */ var _exchange_exchange_component__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./exchange/exchange.component */ "./src/app/exchange/exchange.component.ts");
/* harmony import */ var _exchange_dialog_exchange_dialog_component__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./exchange-dialog/exchange-dialog.component */ "./src/app/exchange-dialog/exchange-dialog.component.ts");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/forms */ "./node_modules/@angular/forms/fesm5/forms.js");
/* harmony import */ var _services_logger_service__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! ./services/logger.service */ "./src/app/services/logger.service.ts");
/* harmony import */ var _services_ws_client_service__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./services/ws-client.service */ "./src/app/services/ws-client.service.ts");
/* harmony import */ var _services_prime_socket_service__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! ./services/prime-socket.service */ "./src/app/services/prime-socket.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};













var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["NgModule"])({
            declarations: [
                _app_component__WEBPACK_IMPORTED_MODULE_4__["AppComponent"],
                _exchanges_exchanges_component__WEBPACK_IMPORTED_MODULE_5__["ExchangesComponent"],
                _toolbar_toolbar_component__WEBPACK_IMPORTED_MODULE_6__["ToolbarComponent"],
                _exchange_exchange_component__WEBPACK_IMPORTED_MODULE_7__["ExchangeComponent"],
                _exchange_dialog_exchange_dialog_component__WEBPACK_IMPORTED_MODULE_8__["ExchangeDialogComponent"],
            ],
            imports: [
                _angular_platform_browser__WEBPACK_IMPORTED_MODULE_0__["BrowserModule"],
                _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_1__["BrowserAnimationsModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_9__["FormsModule"],
                _material_module__WEBPACK_IMPORTED_MODULE_3__["MaterialModule"],
            ],
            providers: [
                _services_logger_service__WEBPACK_IMPORTED_MODULE_10__["LoggerService"],
                { provide: "ISocketClient", useClass: _services_ws_client_service__WEBPACK_IMPORTED_MODULE_11__["WsClientService"] },
                _services_prime_socket_service__WEBPACK_IMPORTED_MODULE_12__["PrimeSocketService"],
            ],
            entryComponents: [
                _exchange_dialog_exchange_dialog_component__WEBPACK_IMPORTED_MODULE_8__["ExchangeDialogComponent"]
            ],
            bootstrap: [_app_component__WEBPACK_IMPORTED_MODULE_4__["AppComponent"]]
        })
    ], AppModule);
    return AppModule;
}());



/***/ }),

/***/ "./src/app/exchange-dialog/exchange-dialog.component.css":
/*!***************************************************************!*\
  !*** ./src/app/exchange-dialog/exchange-dialog.component.css ***!
  \***************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/exchange-dialog/exchange-dialog.component.html":
/*!****************************************************************!*\
  !*** ./src/app/exchange-dialog/exchange-dialog.component.html ***!
  \****************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<h2 mat-dialog-title>{{ exchangeDetails.title }} details</h2>\n<mat-dialog-content>\n\n  <mat-form-field class=\"full-width\">\n    <input matInput [(ngModel)]=\"exchangeDetails.privateApiContext.key\" placeholder=\"Public Key\">\n  </mat-form-field>\n\n  <mat-form-field class=\"full-width\">\n    <input matInput [(ngModel)]=\"exchangeDetails.privateApiContext.secret\" placeholder=\"Secret Key\">\n  </mat-form-field>\n\n  <mat-slide-toggle [(ngModel)]=\"extraEnabled\" >Has Extra data</mat-slide-toggle>\n\n  <mat-form-field class=\"full-width\">\n    <input matInput [disabled]=\"!extraEnabled\" [(ngModel)]=\"exchangeDetails.privateApiContext.extra\" placeholder=\"Extra\">\n  </mat-form-field>\n\n</mat-dialog-content>\n<mat-dialog-actions>\n  <button mat-button [mat-dialog-close]=\"true\">Close</button>\n  <span class=\"fill-space\"></span>\n\n  <button mat-button (click)=\"testPrivateApi()\">Test Private API</button>\n  <button mat-button color=\"warn\">Delete</button>\n  <button mat-raised-button color=\"primary\">Save</button>\n</mat-dialog-actions>"

/***/ }),

/***/ "./src/app/exchange-dialog/exchange-dialog.component.ts":
/*!**************************************************************!*\
  !*** ./src/app/exchange-dialog/exchange-dialog.component.ts ***!
  \**************************************************************/
/*! exports provided: ExchangeDialogComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ExchangeDialogComponent", function() { return ExchangeDialogComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _models_ExchangeDetails__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../models/ExchangeDetails */ "./src/app/models/ExchangeDetails.ts");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var _services_prime_socket_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../services/prime-socket.service */ "./src/app/services/prime-socket.service.ts");
/* harmony import */ var _models_private_api_context__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../models/private-api-context */ "./src/app/models/private-api-context.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var ExchangeDialogComponent = /** @class */ (function () {
    function ExchangeDialogComponent(snackBar, primeSocket) {
        this.snackBar = snackBar;
        this.primeSocket = primeSocket;
        this.extraEnabled = false;
        this.exchangeDetails = new _models_ExchangeDetails__WEBPACK_IMPORTED_MODULE_1__["ExchangeDetails"]("Poloniex", new _models_private_api_context__WEBPACK_IMPORTED_MODULE_4__["PrivateApiContext"]("awdjashdk1j2h3", "Key", "Secret", "Extra"));
    }
    ExchangeDialogComponent.prototype.testPrivateApi = function () {
        if (!this.extraEnabled)
            this.exchangeDetails.privateApiContext.extra = null;
        this.primeSocket.testPrivateApi(this.exchangeDetails.privateApiContext);
        this.snackBar.open("Private API test!", "Info", {
            duration: 3000
        });
    };
    ExchangeDialogComponent.prototype.ngOnInit = function () {
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", _models_ExchangeDetails__WEBPACK_IMPORTED_MODULE_1__["ExchangeDetails"])
    ], ExchangeDialogComponent.prototype, "exchangeDetails", void 0);
    ExchangeDialogComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-exchange-dialog',
            template: __webpack_require__(/*! ./exchange-dialog.component.html */ "./src/app/exchange-dialog/exchange-dialog.component.html"),
            styles: [__webpack_require__(/*! ./exchange-dialog.component.css */ "./src/app/exchange-dialog/exchange-dialog.component.css")]
        }),
        __metadata("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatSnackBar"],
            _services_prime_socket_service__WEBPACK_IMPORTED_MODULE_3__["PrimeSocketService"]])
    ], ExchangeDialogComponent);
    return ExchangeDialogComponent;
}());



/***/ }),

/***/ "./src/app/exchange/exchange.component.css":
/*!*************************************************!*\
  !*** ./src/app/exchange/exchange.component.css ***!
  \*************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "\r\n.exchange-card {\r\n    margin: 10px;\r\n}\r\n\r\n.header .title {\r\n    font-size: 25px;\r\n    margin-bottom: 10px;\r\n}\r\n\r\n.header .description {\r\n    color: rgba(0, 0, 0, 0.6)\r\n}\r\n\r\n.mat-card-actions {\r\n    margin-top: 10px;\r\n}\r\n"

/***/ }),

/***/ "./src/app/exchange/exchange.component.html":
/*!**************************************************!*\
  !*** ./src/app/exchange/exchange.component.html ***!
  \**************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<mat-card class=\"exchange-card\">\n  <mat-card-content class=\"header\">\n    <mat-card-title class=\"title\">\n        {{ exchange.title }}\n    </mat-card-title>\n    <mat-card-subtitle class=\"description\">{{ exchange.exchangeId }}</mat-card-subtitle>\n  </mat-card-content>\n  <mat-card-actions>\n    <button mat-button color=\"primary\" (click)=\"openDialog()\">MANAGE</button>\n  </mat-card-actions>\n</mat-card>"

/***/ }),

/***/ "./src/app/exchange/exchange.component.ts":
/*!************************************************!*\
  !*** ./src/app/exchange/exchange.component.ts ***!
  \************************************************/
/*! exports provided: ExchangeComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ExchangeComponent", function() { return ExchangeComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _models_Exchange__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../models/Exchange */ "./src/app/models/Exchange.ts");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var _exchange_dialog_exchange_dialog_component__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../exchange-dialog/exchange-dialog.component */ "./src/app/exchange-dialog/exchange-dialog.component.ts");
/* harmony import */ var _services_logger_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../services/logger.service */ "./src/app/services/logger.service.ts");
/* harmony import */ var _services_prime_socket_service__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../services/prime-socket.service */ "./src/app/services/prime-socket.service.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var ExchangeComponent = /** @class */ (function () {
    function ExchangeComponent(dialog, primeSocket) {
        this.dialog = dialog;
        this.primeSocket = primeSocket;
    }
    ExchangeComponent.prototype.openDialog = function () {
        _services_logger_service__WEBPACK_IMPORTED_MODULE_4__["LoggerService"].log("Opening dialog...");
        this.primeSocket.getProviderProvidersList();
        var dialogConfig = new _angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialogConfig"]();
        dialogConfig.disableClose = false;
        dialogConfig.autoFocus = true;
        this.dialog.open(_exchange_dialog_exchange_dialog_component__WEBPACK_IMPORTED_MODULE_3__["ExchangeDialogComponent"], dialogConfig);
    };
    ExchangeComponent.prototype.ngOnInit = function () {
    };
    __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Input"])(),
        __metadata("design:type", _models_Exchange__WEBPACK_IMPORTED_MODULE_1__["Exchange"])
    ], ExchangeComponent.prototype, "exchange", void 0);
    ExchangeComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-exchange',
            template: __webpack_require__(/*! ./exchange.component.html */ "./src/app/exchange/exchange.component.html"),
            styles: [__webpack_require__(/*! ./exchange.component.css */ "./src/app/exchange/exchange.component.css")]
        }),
        __metadata("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialog"],
            _services_prime_socket_service__WEBPACK_IMPORTED_MODULE_5__["PrimeSocketService"]])
    ], ExchangeComponent);
    return ExchangeComponent;
}());



/***/ }),

/***/ "./src/app/exchanges/exchanges.component.css":
/*!***************************************************!*\
  !*** ./src/app/exchanges/exchanges.component.css ***!
  \***************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/exchanges/exchanges.component.html":
/*!****************************************************!*\
  !*** ./src/app/exchanges/exchanges.component.html ***!
  \****************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<app-exchange *ngFor=\"let exchange of exchanges\" [exchange]=\"exchange\"></app-exchange>"

/***/ }),

/***/ "./src/app/exchanges/exchanges.component.ts":
/*!**************************************************!*\
  !*** ./src/app/exchanges/exchanges.component.ts ***!
  \**************************************************/
/*! exports provided: ExchangesComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ExchangesComponent", function() { return ExchangesComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _models_Exchange__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../models/Exchange */ "./src/app/models/Exchange.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var ExchangesComponent = /** @class */ (function () {
    function ExchangesComponent() {
        this.exchanges = [
            new _models_Exchange__WEBPACK_IMPORTED_MODULE_1__["Exchange"]("Poloniex", "123awd23"),
            new _models_Exchange__WEBPACK_IMPORTED_MODULE_1__["Exchange"]("Bittrex", "123awd23"),
            new _models_Exchange__WEBPACK_IMPORTED_MODULE_1__["Exchange"]("Binance", "123awd23")
        ];
    }
    ExchangesComponent.prototype.ngOnInit = function () {
    };
    ExchangesComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-exchanges',
            template: __webpack_require__(/*! ./exchanges.component.html */ "./src/app/exchanges/exchanges.component.html"),
            styles: [__webpack_require__(/*! ./exchanges.component.css */ "./src/app/exchanges/exchanges.component.css")]
        }),
        __metadata("design:paramtypes", [])
    ], ExchangesComponent);
    return ExchangesComponent;
}());



/***/ }),

/***/ "./src/app/material.module.ts":
/*!************************************!*\
  !*** ./src/app/material.module.ts ***!
  \************************************/
/*! exports provided: MaterialModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MaterialModule", function() { return MaterialModule; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var _angular_material_card__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material/card */ "./node_modules/@angular/material/esm5/card.es5.js");
/* harmony import */ var _angular_material_badge__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/material/badge */ "./node_modules/@angular/material/esm5/badge.es5.js");
/* harmony import */ var _angular_material_slide_toggle__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/material/slide-toggle */ "./node_modules/@angular/material/esm5/slide-toggle.es5.js");
/* harmony import */ var _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/material/snack-bar */ "./node_modules/@angular/material/esm5/snack-bar.es5.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};









var MaterialModule = /** @class */ (function () {
    function MaterialModule() {
    }
    MaterialModule = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["NgModule"])({
            imports: [
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatButtonModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatToolbarModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatTabsModule"],
                _angular_material_card__WEBPACK_IMPORTED_MODULE_2__["MatCardModule"],
                _angular_material_badge__WEBPACK_IMPORTED_MODULE_3__["MatBadgeModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatDialogModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatFormFieldModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatInputModule"],
                _angular_material_slide_toggle__WEBPACK_IMPORTED_MODULE_4__["MatSlideToggleModule"],
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_5__["MatSnackBarModule"]
            ],
            exports: [
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatButtonModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatToolbarModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatTabsModule"],
                _angular_material_card__WEBPACK_IMPORTED_MODULE_2__["MatCardModule"],
                _angular_material_badge__WEBPACK_IMPORTED_MODULE_3__["MatBadgeModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatDialogModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatFormFieldModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_1__["MatInputModule"],
                _angular_material_slide_toggle__WEBPACK_IMPORTED_MODULE_4__["MatSlideToggleModule"],
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_5__["MatSnackBarModule"]
            ],
        })
    ], MaterialModule);
    return MaterialModule;
}());



/***/ }),

/***/ "./src/app/models/Exchange.ts":
/*!************************************!*\
  !*** ./src/app/models/Exchange.ts ***!
  \************************************/
/*! exports provided: Exchange */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Exchange", function() { return Exchange; });
var Exchange = /** @class */ (function () {
    function Exchange(title, exchangeId) {
        this.title = title;
        this.exchangeId = exchangeId;
    }
    return Exchange;
}());



/***/ }),

/***/ "./src/app/models/ExchangeDetails.ts":
/*!*******************************************!*\
  !*** ./src/app/models/ExchangeDetails.ts ***!
  \*******************************************/
/*! exports provided: ExchangeDetails */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ExchangeDetails", function() { return ExchangeDetails; });
/* harmony import */ var _Exchange__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./Exchange */ "./src/app/models/Exchange.ts");
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();

var ExchangeDetails = /** @class */ (function (_super) {
    __extends(ExchangeDetails, _super);
    function ExchangeDetails(title, privateApiContext) {
        var _this = _super.call(this, title, privateApiContext.exchangeId) || this;
        _this.privateApiContext = privateApiContext;
        return _this;
    }
    return ExchangeDetails;
}(_Exchange__WEBPACK_IMPORTED_MODULE_0__["Exchange"]));



/***/ }),

/***/ "./src/app/models/messages.ts":
/*!************************************!*\
  !*** ./src/app/models/messages.ts ***!
  \************************************/
/*! exports provided: BaseMessage, UserMessageRequest, ProvidersListMessageRequest, TestPrivateApiMessageRequest, TestPrivateApiMessageResponse, ProviderKeysMessageRequest, ProviderKeysMessageResponse */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "BaseMessage", function() { return BaseMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UserMessageRequest", function() { return UserMessageRequest; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProvidersListMessageRequest", function() { return ProvidersListMessageRequest; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TestPrivateApiMessageRequest", function() { return TestPrivateApiMessageRequest; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TestPrivateApiMessageResponse", function() { return TestPrivateApiMessageResponse; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProviderKeysMessageRequest", function() { return ProviderKeysMessageRequest; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProviderKeysMessageResponse", function() { return ProviderKeysMessageResponse; });
var __extends = (undefined && undefined.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var BaseMessage = /** @class */ (function () {
    function BaseMessage() {
    }
    BaseMessage.prototype.serialize = function () {
        return JSON.stringify(this, function (key, value) {
            if (value !== null)
                return value;
        });
    };
    return BaseMessage;
}());

var UserMessageRequest = /** @class */ (function (_super) {
    __extends(UserMessageRequest, _super);
    function UserMessageRequest() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Type = UserMessageRequest.name;
        return _this;
    }
    return UserMessageRequest;
}(BaseMessage));

var ProvidersListMessageRequest = /** @class */ (function (_super) {
    __extends(ProvidersListMessageRequest, _super);
    function ProvidersListMessageRequest() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Type = ProvidersListMessageRequest.name;
        return _this;
    }
    return ProvidersListMessageRequest;
}(BaseMessage));

var TestPrivateApiMessageRequest = /** @class */ (function (_super) {
    __extends(TestPrivateApiMessageRequest, _super);
    function TestPrivateApiMessageRequest(ExchangeId, Key, Secret, Extra) {
        var _this = _super.call(this) || this;
        _this.ExchangeId = ExchangeId;
        _this.Key = Key;
        _this.Secret = Secret;
        _this.Extra = Extra;
        _this.Type = TestPrivateApiMessageRequest.name;
        return _this;
    }
    return TestPrivateApiMessageRequest;
}(BaseMessage));

var TestPrivateApiMessageResponse = /** @class */ (function () {
    function TestPrivateApiMessageResponse() {
    }
    return TestPrivateApiMessageResponse;
}());

var ProviderKeysMessageRequest = /** @class */ (function (_super) {
    __extends(ProviderKeysMessageRequest, _super);
    function ProviderKeysMessageRequest() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Type = ProviderKeysMessageRequest.name;
        return _this;
    }
    return ProviderKeysMessageRequest;
}(BaseMessage));

var ProviderKeysMessageResponse = /** @class */ (function () {
    function ProviderKeysMessageResponse() {
    }
    return ProviderKeysMessageResponse;
}());



/***/ }),

/***/ "./src/app/models/private-api-context.ts":
/*!***********************************************!*\
  !*** ./src/app/models/private-api-context.ts ***!
  \***********************************************/
/*! exports provided: PrivateApiContext */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PrivateApiContext", function() { return PrivateApiContext; });
var PrivateApiContext = /** @class */ (function () {
    function PrivateApiContext(exchangeId, key, secret, extra) {
        this.exchangeId = exchangeId;
        this.key = key;
        this.secret = secret;
        this.extra = extra;
    }
    return PrivateApiContext;
}());



/***/ }),

/***/ "./src/app/models/socket-state.ts":
/*!****************************************!*\
  !*** ./src/app/models/socket-state.ts ***!
  \****************************************/
/*! exports provided: SocketState */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SocketState", function() { return SocketState; });
var SocketState;
(function (SocketState) {
    SocketState[SocketState["Disconnected"] = 0] = "Disconnected";
    SocketState[SocketState["Connected"] = 1] = "Connected";
})(SocketState || (SocketState = {}));


/***/ }),

/***/ "./src/app/services/logger.service.ts":
/*!********************************************!*\
  !*** ./src/app/services/logger.service.ts ***!
  \********************************************/
/*! exports provided: LoggerService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LoggerService", function() { return LoggerService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var LoggerService = /** @class */ (function () {
    function LoggerService() {
    }
    LoggerService.log = function (message) {
        console.log(message);
    };
    LoggerService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])({
            providedIn: 'root'
        }),
        __metadata("design:paramtypes", [])
    ], LoggerService);
    return LoggerService;
}());



/***/ }),

/***/ "./src/app/services/prime-socket.service.ts":
/*!**************************************************!*\
  !*** ./src/app/services/prime-socket.service.ts ***!
  \**************************************************/
/*! exports provided: PrimeSocketService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PrimeSocketService", function() { return PrimeSocketService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _logger_service__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./logger.service */ "./src/app/services/logger.service.ts");
/* harmony import */ var _models_socket_state__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../models/socket-state */ "./src/app/models/socket-state.ts");
/* harmony import */ var _models_messages__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../models/messages */ "./src/app/models/messages.ts");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (undefined && undefined.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};




var PrimeSocketService = /** @class */ (function () {
    function PrimeSocketService(socketClient) {
        this.socketClient = socketClient;
        this.socketState = _models_socket_state__WEBPACK_IMPORTED_MODULE_2__["SocketState"].Disconnected;
    }
    PrimeSocketService.prototype.connect = function () {
        var _this = this;
        _logger_service__WEBPACK_IMPORTED_MODULE_1__["LoggerService"].log("Starting TCP client...");
        this.socketClient.onClientConnected = function () {
            _logger_service__WEBPACK_IMPORTED_MODULE_1__["LoggerService"].log("Connected to Prime API server.");
            _this.socketState = _models_socket_state__WEBPACK_IMPORTED_MODULE_2__["SocketState"].Connected;
        };
        this.socketClient.onDataReceived = function (data) {
            _logger_service__WEBPACK_IMPORTED_MODULE_1__["LoggerService"].log("Client received data: " + data.data);
        };
        this.socketClient.onConnectionClosed = function () {
            _logger_service__WEBPACK_IMPORTED_MODULE_1__["LoggerService"].log("Connection closed.");
            _this.socketState = _models_socket_state__WEBPACK_IMPORTED_MODULE_2__["SocketState"].Disconnected;
        };
        this.socketClient.connect('ws://0.0.0.0:8081/echo');
    };
    PrimeSocketService.prototype.writeSocket = function (data) {
        this.socketClient.write(data);
    };
    PrimeSocketService.prototype.writeSocketMessage = function (data) {
        this.writeSocket(data.serialize());
    };
    PrimeSocketService.prototype.test = function () {
        this.writeSocket("Hello");
    };
    PrimeSocketService.prototype.getProviderProvidersList = function () {
        this.writeSocketMessage(new _models_messages__WEBPACK_IMPORTED_MODULE_3__["ProvidersListMessageRequest"]());
    };
    PrimeSocketService.prototype.testPrivateApi = function (privateApiContext) {
        this.writeSocketMessage(new _models_messages__WEBPACK_IMPORTED_MODULE_3__["TestPrivateApiMessageRequest"](privateApiContext.exchangeId, privateApiContext.key, privateApiContext.secret, privateApiContext.extra));
    };
    PrimeSocketService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])({
            providedIn: 'root'
        }),
        __param(0, Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Inject"])("ISocketClient")),
        __metadata("design:paramtypes", [Object])
    ], PrimeSocketService);
    return PrimeSocketService;
}());



/***/ }),

/***/ "./src/app/services/ws-client.service.ts":
/*!***********************************************!*\
  !*** ./src/app/services/ws-client.service.ts ***!
  \***********************************************/
/*! exports provided: WsClientService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "WsClientService", function() { return WsClientService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var WsClientService = /** @class */ (function () {
    function WsClientService() {
    }
    WsClientService.prototype.connect = function (host) {
        this.ws = new WebSocket(host);
        this.ws.onopen = this.onClientConnected;
        this.ws.onmessage = this.onDataReceived;
        this.ws.onclose = this.onConnectionClosed;
    };
    WsClientService.prototype.write = function (data) {
        this.ws.send(data);
    };
    WsClientService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])({
            providedIn: 'root'
        }),
        __metadata("design:paramtypes", [])
    ], WsClientService);
    return WsClientService;
}());



/***/ }),

/***/ "./src/app/toolbar/toolbar.component.css":
/*!***********************************************!*\
  !*** ./src/app/toolbar/toolbar.component.css ***!
  \***********************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "\r\n.mat-toolbar {\r\n    position: fixed;\r\n    z-index: 2;\r\n    top: 0;\r\n}"

/***/ }),

/***/ "./src/app/toolbar/toolbar.component.html":
/*!************************************************!*\
  !*** ./src/app/toolbar/toolbar.component.html ***!
  \************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "\n<mat-toolbar color=\"primary\">\n  <span>{{title}}</span>\n</mat-toolbar>\n\n"

/***/ }),

/***/ "./src/app/toolbar/toolbar.component.ts":
/*!**********************************************!*\
  !*** ./src/app/toolbar/toolbar.component.ts ***!
  \**********************************************/
/*! exports provided: ToolbarComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ToolbarComponent", function() { return ToolbarComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (undefined && undefined.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var ToolbarComponent = /** @class */ (function () {
    function ToolbarComponent() {
        this.title = "Prime.KeysManager";
    }
    ToolbarComponent.prototype.ngOnInit = function () {
    };
    ToolbarComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-toolbar',
            template: __webpack_require__(/*! ./toolbar.component.html */ "./src/app/toolbar/toolbar.component.html"),
            styles: [__webpack_require__(/*! ./toolbar.component.css */ "./src/app/toolbar/toolbar.component.css")]
        }),
        __metadata("design:paramtypes", [])
    ], ToolbarComponent);
    return ToolbarComponent;
}());



/***/ }),

/***/ "./src/environments/environment.ts":
/*!*****************************************!*\
  !*** ./src/environments/environment.ts ***!
  \*****************************************/
/*! exports provided: environment */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "environment", function() { return environment; });
// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.
var environment = {
    production: false
};
/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.


/***/ }),

/***/ "./src/main.ts":
/*!*********************!*\
  !*** ./src/main.ts ***!
  \*********************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var hammerjs__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! hammerjs */ "./node_modules/hammerjs/hammer.js");
/* harmony import */ var hammerjs__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(hammerjs__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/platform-browser-dynamic */ "./node_modules/@angular/platform-browser-dynamic/fesm5/platform-browser-dynamic.js");
/* harmony import */ var _app_app_module__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./app/app.module */ "./src/app/app.module.ts");
/* harmony import */ var _environments_environment__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./environments/environment */ "./src/environments/environment.ts");





if (_environments_environment__WEBPACK_IMPORTED_MODULE_4__["environment"].production) {
    Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["enableProdMode"])();
}
Object(_angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_2__["platformBrowserDynamic"])().bootstrapModule(_app_app_module__WEBPACK_IMPORTED_MODULE_3__["AppModule"])
    .catch(function (err) { return console.log(err); });


/***/ }),

/***/ 0:
/*!***************************!*\
  !*** multi ./src/main.ts ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(/*! /Users/alexander/Desktop/AngularElectron/src/main.ts */"./src/main.ts");


/***/ })

},[[0,"runtime","vendor"]]]);
//# sourceMappingURL=main.js.map
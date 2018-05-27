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

module.exports = "<!--The content below is only a placeholder and can be replaced.-->\r\n\r\n<app-toolbar></app-toolbar>\r\n\r\n<mat-tab-group>\r\n    <mat-tab label=\"Exchanges\">\r\n        <div class=\"tab-content tab-exchanges\">\r\n            <app-exchanges></app-exchanges>\r\n        </div>\r\n\r\n    </mat-tab>\r\n    <mat-tab label=\"Chart\">\r\n        <div class=\"tab-content tab-chart\">\r\n            Hello\r\n        </div>\r\n    </mat-tab>\r\n</mat-tab-group>"

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
    }
    AppComponent.prototype.ngOnInit = function () {
    };
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
/* harmony import */ var _pipes_filter_pipe__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! ./pipes/filter.pipe */ "./src/app/pipes/filter.pipe.ts");
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
                _pipes_filter_pipe__WEBPACK_IMPORTED_MODULE_13__["FilterPipe"]
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

module.exports = "<h2 mat-dialog-title>{{ exchangeDetails.name }} details</h2>\r\n<mat-dialog-content>\r\n\r\n  <mat-form-field class=\"full-width\">\r\n    <input matInput [(ngModel)]=\"exchangeDetails.privateApiContext.key\" placeholder=\"Public Key\">\r\n    <button mat-button *ngIf=\"exchangeDetails.privateApiContext.key\" matSuffix mat-icon-button aria-label=\"Clear\" (click)=\"exchangeDetails.privateApiContext.key=''\">\r\n      <mat-icon>close</mat-icon>\r\n    </button>\r\n  </mat-form-field>\r\n\r\n  <mat-form-field class=\"full-width\">\r\n    <input matInput type=\"password\" [(ngModel)]=\"exchangeDetails.privateApiContext.secret\" placeholder=\"Secret Key\">\r\n    <button mat-button *ngIf=\"exchangeDetails.privateApiContext.secret\" matSuffix mat-icon-button aria-label=\"Clear\" (click)=\"exchangeDetails.privateApiContext.secret=''\">\r\n      <mat-icon>close</mat-icon>\r\n    </button>\r\n  </mat-form-field>\r\n\r\n  <mat-slide-toggle (change)=\"extraChanged($event)\" [(ngModel)]=\"extraEnabled\">Has Extra data</mat-slide-toggle>\r\n\r\n  <mat-form-field class=\"full-width\">\r\n    <input matInput [disabled]=\"!extraEnabled\" [(ngModel)]=\"exchangeDetails.privateApiContext.extra\" placeholder=\"Extra\">\r\n    <button mat-button *ngIf=\"exchangeDetails.privateApiContext.extra\" matSuffix mat-icon-button aria-label=\"Clear\" (click)=\"exchangeDetails.privateApiContext.extra=''\">\r\n      <mat-icon>close</mat-icon>\r\n    </button>\r\n  </mat-form-field>\r\n\r\n</mat-dialog-content>\r\n<mat-dialog-actions>\r\n  <button mat-button [mat-dialog-close]=\"true\">Close</button>\r\n  <span class=\"fill-space\"></span>\r\n\r\n  <button mat-button (click)=\"testPrivateApi()\">Test Private API</button>\r\n  <button mat-button (click)=\"deleteKeys()\" color=\"warn\">Delete</button>\r\n  <button mat-raised-button color=\"primary\" [mat-dialog-close]=\"true\" (click)=\"saveApiKeys()\">Save</button>\r\n</mat-dialog-actions>"

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
/* harmony import */ var _models_Exchange__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../models/Exchange */ "./src/app/models/Exchange.ts");
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






var ExchangeDialogComponent = /** @class */ (function () {
    function ExchangeDialogComponent(snackBar, primeSocket, data) {
        this.snackBar = snackBar;
        this.primeSocket = primeSocket;
        this.data = data;
        this.extraEnabled = false;
        this.exchangeDetails = new _models_ExchangeDetails__WEBPACK_IMPORTED_MODULE_1__["ExchangeDetails"]("Provider", new _models_private_api_context__WEBPACK_IMPORTED_MODULE_4__["PrivateApiContext"]("", "", ""));
        //this.exchangeDetails = data;
        //LoggerService.logObj(data);
    }
    ExchangeDialogComponent.prototype.resetExchangeDetails = function () {
        if (this.exchangeDetails.privateApiContext === null) {
            this.exchangeDetails.privateApiContext = new _models_private_api_context__WEBPACK_IMPORTED_MODULE_4__["PrivateApiContext"]("", "", "");
        }
        else {
            this.exchangeDetails.privateApiContext.key = "";
            this.exchangeDetails.privateApiContext.secret = "";
            this.exchangeDetails.privateApiContext.extra = "";
        }
    };
    ExchangeDialogComponent.prototype.testPrivateApi = function () {
        var _this = this;
        if (!this.extraEnabled)
            this.exchangeDetails.privateApiContext.extra = null;
        this.primeSocket.testPrivateApi(this.exchangeDetails.privateApiContext, function (data) {
            _this.snackBar.open((data.success ? "API test succeeded" : (data.message != "" && data.message != null ? "API test error: " + data.message : "API test error occurred")), "Info", {
                duration: 3000
            });
        });
    };
    ExchangeDialogComponent.prototype.extraChanged = function (event) {
        if (this.extraEnabled === false) {
            this.exchangeDetails.privateApiContext.extra = null;
        }
    };
    ExchangeDialogComponent.prototype.saveApiKeys = function () {
        var _this = this;
        if (this.extraEnabled === false) {
            this.exchangeDetails.privateApiContext.extra = null;
        }
        this.primeSocket.saveApiKeys(this.exchangeDetails, function (data) {
            _this.snackBar.open((data.success ? "Keys saved" : "Error during saving: " + data.message), "Info", {
                duration: 3000
            });
        });
    };
    ExchangeDialogComponent.prototype.deleteKeys = function () {
        var _this = this;
        this.primeSocket.deleteKeys(this.exchangeDetails.id, function (data) {
            _this.resetExchangeDetails();
            _this.snackBar.open((data.success ? "Keys deleted" : "Error during saving: " + data.message), "Info", {
                duration: 3000
            });
        });
    };
    ExchangeDialogComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.primeSocket.getProviderDetails(this.data.id, function (data) {
            _this.exchangeDetails = new _models_ExchangeDetails__WEBPACK_IMPORTED_MODULE_1__["ExchangeDetails"](data.response.name, new _models_private_api_context__WEBPACK_IMPORTED_MODULE_4__["PrivateApiContext"](data.response.id, data.response.key, data.response.secret, data.response.extra));
            if (data.response.extra !== null && data.response.extra !== "") {
                _this.extraEnabled = true;
            }
        });
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
        __param(2, Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Inject"])(_angular_material__WEBPACK_IMPORTED_MODULE_2__["MAT_DIALOG_DATA"])),
        __metadata("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatSnackBar"],
            _services_prime_socket_service__WEBPACK_IMPORTED_MODULE_3__["PrimeSocketService"],
            _models_Exchange__WEBPACK_IMPORTED_MODULE_5__["Exchange"]])
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

module.exports = "\r\n.exchange-card {\r\n    margin: 10px 0;\r\n}\r\n\r\n.header .title {\r\n    font-size: 25px;\r\n    margin-bottom: 10px;\r\n}\r\n\r\n.header .description {\r\n    color: rgba(0, 0, 0, 0.6)\r\n}\r\n\r\n.mat-card-actions {\r\n    margin-top: 10px;\r\n}\r\n\r\n.icon-has-keys {\r\n    float: right;\r\n    color: gray;\r\n}\r\n"

/***/ }),

/***/ "./src/app/exchange/exchange.component.html":
/*!**************************************************!*\
  !*** ./src/app/exchange/exchange.component.html ***!
  \**************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<mat-card class=\"exchange-card\">\r\n  <mat-card-content class=\"header\">\r\n    <mat-card-title class=\"title\">\r\n        {{ exchange.name }}\r\n        <mat-icon *ngIf=\"exchange.hasKeys\" class=\"icon-has-keys\">vpn_key</mat-icon>\r\n    </mat-card-title>\r\n    <mat-card-subtitle class=\"description\">{{ exchange.id }}</mat-card-subtitle>\r\n  </mat-card-content>\r\n  <mat-card-actions>\r\n    <button mat-button color=\"primary\" (click)=\"openDialog(exchange.id)\">MANAGE</button>\r\n  </mat-card-actions>\r\n</mat-card>"

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
    ExchangeComponent.prototype.openDialog = function (idHash) {
        var _this = this;
        _services_logger_service__WEBPACK_IMPORTED_MODULE_4__["LoggerService"].log("Opening dialog (" + this.exchange.id + ")...");
        var dialogConfig = new _angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialogConfig"]();
        dialogConfig.disableClose = false;
        dialogConfig.autoFocus = true;
        dialogConfig.data = this.exchange;
        dialogConfig.width = "535px";
        var dialog = this.dialog.open(_exchange_dialog_exchange_dialog_component__WEBPACK_IMPORTED_MODULE_3__["ExchangeDialogComponent"], dialogConfig);
        dialog.beforeClose().subscribe(function (r) {
            _this.primeSocket.checkProvidersKeys(idHash, function (data) {
                _this.exchange.hasKeys = data.success;
            });
        });
    };
    ExchangeComponent.prototype.ngOnInit = function () {
        this.primeSocket.onClientConnected = function () {
            console.log("Overried connection");
        };
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

module.exports = "<mat-form-field class=\"full-width exchange-search\">\r\n    <input matInput type=\"text\" placeholder=\"Search exchange\" [(ngModel)]=\"exchangeFilter\" />\r\n    <button mat-button *ngIf=\"exchangeFilter\" matSuffix mat-icon-button aria-label=\"Clear\" (click)=\"exchangeFilter=''\">\r\n        <mat-icon>close</mat-icon>\r\n    </button>\r\n</mat-form-field>\r\n\r\n<app-exchange *ngFor=\"let exchange of (exchanges | filter: exchangeFilter)\" [exchange]=\"exchange\"></app-exchange>"

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
/* harmony import */ var _services_prime_socket_service__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../services/prime-socket.service */ "./src/app/services/prime-socket.service.ts");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var _services_action_throttler_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../services/action-throttler.service */ "./src/app/services/action-throttler.service.ts");
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
    function ExchangesComponent(primeService, actionThrottler, snackBar) {
        var _this = this;
        this.primeService = primeService;
        this.actionThrottler = actionThrottler;
        this.snackBar = snackBar;
        primeService.onClientConnected = function () {
            _this.snackBar.open("Connected to server", "Info", {
                duration: 3000
            });
            _this.primeService.getPrivateProvidersList(function (data) {
                _this.exchanges = data.response;
            });
        };
        primeService.onErrorOccurred = function () {
            _this.snackBar.open("Connection error occurred", "Error", {
                duration: 3000
            });
        };
    }
    ExchangesComponent.prototype.ngOnInit = function () {
        this.primeService.connect();
    };
    ExchangesComponent = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Component"])({
            selector: 'app-exchanges',
            template: __webpack_require__(/*! ./exchanges.component.html */ "./src/app/exchanges/exchanges.component.html"),
            styles: [__webpack_require__(/*! ./exchanges.component.css */ "./src/app/exchanges/exchanges.component.css")]
        }),
        __metadata("design:paramtypes", [_services_prime_socket_service__WEBPACK_IMPORTED_MODULE_1__["PrimeSocketService"],
            _services_action_throttler_service__WEBPACK_IMPORTED_MODULE_3__["ActionThrottlerService"],
            _angular_material__WEBPACK_IMPORTED_MODULE_2__["MatSnackBar"]])
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
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/icon */ "./node_modules/@angular/material/esm5/icon.es5.js");
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
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_5__["MatSnackBarModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__["MatIconModule"]
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
                _angular_material_snack_bar__WEBPACK_IMPORTED_MODULE_5__["MatSnackBarModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_6__["MatIconModule"]
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
    function Exchange(name, id, hasKeys) {
        if (hasKeys === void 0) { hasKeys = false; }
        this.name = name;
        this.id = id;
        this.hasKeys = hasKeys;
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
/*! exports provided: BaseMessage, BaseRequestMessage, BaseResponseMessage, BooleanResponseMessage, UserMessageRequest, ProvidersListRequestMessage, ProvidersListResponseMessage, PrivateProvidersListRequestMessage, PrivateProvidersListResponseMessage, DeleteProviderKeysRequestMessage, DeleteProviderKeysResponseMessage, ProviderDetailsRequestMessage, ProviderDetailsResponseMessage, ProviderSaveKeysRequestMessage, ProviderSaveKeysResponseMessage, TestPrivateApiRequestMessage, TestPrivateApiResponseMessage, ProviderHasKeysRequestMessage, ProviderHasKeysResponseMessage, ProviderKeysMessageRequest, ProviderKeysMessageResponse */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "BaseMessage", function() { return BaseMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "BaseRequestMessage", function() { return BaseRequestMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "BaseResponseMessage", function() { return BaseResponseMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "BooleanResponseMessage", function() { return BooleanResponseMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UserMessageRequest", function() { return UserMessageRequest; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProvidersListRequestMessage", function() { return ProvidersListRequestMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProvidersListResponseMessage", function() { return ProvidersListResponseMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PrivateProvidersListRequestMessage", function() { return PrivateProvidersListRequestMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PrivateProvidersListResponseMessage", function() { return PrivateProvidersListResponseMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DeleteProviderKeysRequestMessage", function() { return DeleteProviderKeysRequestMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DeleteProviderKeysResponseMessage", function() { return DeleteProviderKeysResponseMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProviderDetailsRequestMessage", function() { return ProviderDetailsRequestMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProviderDetailsResponseMessage", function() { return ProviderDetailsResponseMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProviderSaveKeysRequestMessage", function() { return ProviderSaveKeysRequestMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProviderSaveKeysResponseMessage", function() { return ProviderSaveKeysResponseMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TestPrivateApiRequestMessage", function() { return TestPrivateApiRequestMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TestPrivateApiResponseMessage", function() { return TestPrivateApiResponseMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProviderHasKeysRequestMessage", function() { return ProviderHasKeysRequestMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ProviderHasKeysResponseMessage", function() { return ProviderHasKeysResponseMessage; });
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
            if (value !== null && key !== "expectedEmptyResponse")
                return value;
        });
    };
    return BaseMessage;
}());

var BaseRequestMessage = /** @class */ (function (_super) {
    __extends(BaseRequestMessage, _super);
    function BaseRequestMessage() {
        var _this = _super.call(this) || this;
        _this.expectedEmptyResponse = _this.createExpectedEmptyResponse();
        return _this;
    }
    return BaseRequestMessage;
}(BaseMessage));

var BaseResponseMessage = /** @class */ (function (_super) {
    __extends(BaseResponseMessage, _super);
    function BaseResponseMessage() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return BaseResponseMessage;
}(BaseMessage));

var BooleanResponseMessage = /** @class */ (function (_super) {
    __extends(BooleanResponseMessage, _super);
    function BooleanResponseMessage() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return BooleanResponseMessage;
}(BaseResponseMessage));

var UserMessageRequest = /** @class */ (function (_super) {
    __extends(UserMessageRequest, _super);
    function UserMessageRequest() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = UserMessageRequest.name;
        return _this;
    }
    return UserMessageRequest;
}(BaseMessage));

var ProvidersListRequestMessage = /** @class */ (function (_super) {
    __extends(ProvidersListRequestMessage, _super);
    function ProvidersListRequestMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.providerslistrequestmessage";
        return _this;
    }
    return ProvidersListRequestMessage;
}(BaseMessage));

var ProvidersListResponseMessage = /** @class */ (function (_super) {
    __extends(ProvidersListResponseMessage, _super);
    function ProvidersListResponseMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.providerslistresponsemessage";
        return _this;
    }
    return ProvidersListResponseMessage;
}(BaseResponseMessage));

var PrivateProvidersListRequestMessage = /** @class */ (function (_super) {
    __extends(PrivateProvidersListRequestMessage, _super);
    function PrivateProvidersListRequestMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.privateproviderslistrequestmessage";
        return _this;
    }
    PrivateProvidersListRequestMessage.prototype.createExpectedEmptyResponse = function () {
        return new PrivateProvidersListResponseMessage();
    };
    return PrivateProvidersListRequestMessage;
}(BaseRequestMessage));

var PrivateProvidersListResponseMessage = /** @class */ (function (_super) {
    __extends(PrivateProvidersListResponseMessage, _super);
    function PrivateProvidersListResponseMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.privateproviderslistresponsemessage";
        return _this;
    }
    return PrivateProvidersListResponseMessage;
}(BaseResponseMessage));

var DeleteProviderKeysRequestMessage = /** @class */ (function (_super) {
    __extends(DeleteProviderKeysRequestMessage, _super);
    function DeleteProviderKeysRequestMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.deleteproviderkeysrequestmessage";
        return _this;
    }
    DeleteProviderKeysRequestMessage.prototype.createExpectedEmptyResponse = function () {
        return new DeleteProviderKeysResponseMessage();
    };
    return DeleteProviderKeysRequestMessage;
}(BaseRequestMessage));

var DeleteProviderKeysResponseMessage = /** @class */ (function (_super) {
    __extends(DeleteProviderKeysResponseMessage, _super);
    function DeleteProviderKeysResponseMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.deleteproviderkeysresponsemessage";
        return _this;
    }
    return DeleteProviderKeysResponseMessage;
}(BooleanResponseMessage));

var ProviderDetailsRequestMessage = /** @class */ (function (_super) {
    __extends(ProviderDetailsRequestMessage, _super);
    function ProviderDetailsRequestMessage(id) {
        var _this = _super.call(this) || this;
        _this.$type = "prime.manager.providerdetailsrequestmessage";
        _this.id = id;
        return _this;
    }
    ProviderDetailsRequestMessage.prototype.createExpectedEmptyResponse = function () {
        return new ProviderDetailsResponseMessage();
    };
    return ProviderDetailsRequestMessage;
}(BaseRequestMessage));

var ProviderDetailsResponseMessage = /** @class */ (function (_super) {
    __extends(ProviderDetailsResponseMessage, _super);
    function ProviderDetailsResponseMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.providerdetailsresponsemessage";
        return _this;
    }
    return ProviderDetailsResponseMessage;
}(BaseResponseMessage));

var ProviderSaveKeysRequestMessage = /** @class */ (function (_super) {
    __extends(ProviderSaveKeysRequestMessage, _super);
    function ProviderSaveKeysRequestMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.providersavekeysrequestmessage";
        return _this;
    }
    ProviderSaveKeysRequestMessage.prototype.createExpectedEmptyResponse = function () {
        return new ProviderSaveKeysResponseMessage();
    };
    return ProviderSaveKeysRequestMessage;
}(BaseRequestMessage));

var ProviderSaveKeysResponseMessage = /** @class */ (function (_super) {
    __extends(ProviderSaveKeysResponseMessage, _super);
    function ProviderSaveKeysResponseMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.providersavekeysresponsemessage";
        return _this;
    }
    return ProviderSaveKeysResponseMessage;
}(BooleanResponseMessage));

var TestPrivateApiRequestMessage = /** @class */ (function (_super) {
    __extends(TestPrivateApiRequestMessage, _super);
    function TestPrivateApiRequestMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.testprivateapirequestmessage";
        return _this;
    }
    TestPrivateApiRequestMessage.prototype.createExpectedEmptyResponse = function () {
        return new TestPrivateApiResponseMessage();
    };
    return TestPrivateApiRequestMessage;
}(BaseRequestMessage));

var TestPrivateApiResponseMessage = /** @class */ (function (_super) {
    __extends(TestPrivateApiResponseMessage, _super);
    function TestPrivateApiResponseMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.testprivateapiresponsemessage";
        return _this;
    }
    return TestPrivateApiResponseMessage;
}(BooleanResponseMessage));

var ProviderHasKeysRequestMessage = /** @class */ (function (_super) {
    __extends(ProviderHasKeysRequestMessage, _super);
    function ProviderHasKeysRequestMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.providerhaskeysrequestmessage";
        return _this;
    }
    ProviderHasKeysRequestMessage.prototype.createExpectedEmptyResponse = function () {
        return new ProviderHasKeysResponseMessage();
    };
    return ProviderHasKeysRequestMessage;
}(BaseRequestMessage));

var ProviderHasKeysResponseMessage = /** @class */ (function (_super) {
    __extends(ProviderHasKeysResponseMessage, _super);
    function ProviderHasKeysResponseMessage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = "prime.manager.providerhaskeysresponsemessage";
        return _this;
    }
    return ProviderHasKeysResponseMessage;
}(BooleanResponseMessage));

var ProviderKeysMessageRequest = /** @class */ (function (_super) {
    __extends(ProviderKeysMessageRequest, _super);
    function ProviderKeysMessageRequest() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.$type = ProviderKeysMessageRequest.name;
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

/***/ "./src/app/pipes/filter.pipe.ts":
/*!**************************************!*\
  !*** ./src/app/pipes/filter.pipe.ts ***!
  \**************************************/
/*! exports provided: FilterPipe */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FilterPipe", function() { return FilterPipe; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var FilterPipe = /** @class */ (function () {
    function FilterPipe() {
    }
    FilterPipe.prototype.transform = function (value, args) {
        var exchanges = value;
        var filterString = args;
        if (filterString === undefined || (filterString !== undefined && filterString.length === 0))
            return exchanges;
        if (filterString !== undefined)
            filterString = filterString.toLowerCase();
        return exchanges.filter(function (exchange) { return exchange.name.toLowerCase().indexOf(filterString) !== -1; });
    };
    FilterPipe = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Pipe"])({
            name: 'filter'
        })
    ], FilterPipe);
    return FilterPipe;
}());



/***/ }),

/***/ "./src/app/services/action-throttler.service.ts":
/*!******************************************************!*\
  !*** ./src/app/services/action-throttler.service.ts ***!
  \******************************************************/
/*! exports provided: ActionThrottlerService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ActionThrottlerService", function() { return ActionThrottlerService; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
var __decorate = (undefined && undefined.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var ActionThrottlerService = /** @class */ (function () {
    function ActionThrottlerService() {
        this.timer = null;
    }
    ActionThrottlerService.prototype.throttle = function (period, action) {
        clearTimeout(this.timer);
        this.timer = setTimeout(function () {
            action();
        }, period);
    };
    ActionThrottlerService = __decorate([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["Injectable"])({
            providedIn: 'root'
        })
    ], ActionThrottlerService);
    return ActionThrottlerService;
}());



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
    LoggerService.logObj = function (object) {
        console.log(object);
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
        this.callbacks = [];
        // TODO: implement multiple subscriptions.
        this.onClientConnected = null;
        this.onConnectionClosed = null;
        this.onErrorOccurred = null;
        this.socketState = _models_socket_state__WEBPACK_IMPORTED_MODULE_2__["SocketState"].Disconnected;
    }
    // Connection.
    PrimeSocketService.prototype.connect = function () {
        var _this = this;
        _logger_service__WEBPACK_IMPORTED_MODULE_1__["LoggerService"].log("Starting TCP client...");
        this.socketClient.onClientConnected = function () {
            _logger_service__WEBPACK_IMPORTED_MODULE_1__["LoggerService"].log("Connected to Prime API server.");
            _this.socketState = _models_socket_state__WEBPACK_IMPORTED_MODULE_2__["SocketState"].Connected;
            if (_this.onClientConnected !== null) {
                _this.onClientConnected();
            }
        };
        this.socketClient.onDataReceived = function (data) {
            var response = JSON.parse(data.data);
            if (_this.callbacks[response.$type] != undefined) {
                _this.callbacks[response.$type](response);
                delete _this.callbacks[response.$type];
            }
            else {
                throw "Callback method is not found for " + response.$type;
            }
        };
        this.socketClient.onConnectionClosed = function () {
            _logger_service__WEBPACK_IMPORTED_MODULE_1__["LoggerService"].log("Connection closed.");
            _this.socketState = _models_socket_state__WEBPACK_IMPORTED_MODULE_2__["SocketState"].Disconnected;
            if (_this.onConnectionClosed !== null) {
                _this.onConnectionClosed();
            }
        };
        this.socketClient.onErrorOccurred = function () {
            if (_this.onErrorOccurred !== null) {
                _this.onErrorOccurred();
            }
        };
        this.socketClient.connect('ws://127.0.0.1:9991/');
    };
    // Socket messaging.
    PrimeSocketService.prototype.writeSocket = function (data, callback) {
        this.socketClient.write(data);
    };
    PrimeSocketService.prototype.writeSocketMessage = function (data, callback) {
        this.callbacks[data.expectedEmptyResponse.$type] = callback;
        this.writeSocket(data.serialize(), callback);
    };
    // Core logic methods.
    PrimeSocketService.prototype.test = function () {
        this.writeSocket("Hello");
    };
    PrimeSocketService.prototype.getPrivateProvidersList = function (callback) {
        var x = new _models_messages__WEBPACK_IMPORTED_MODULE_3__["PrivateProvidersListRequestMessage"]();
        this.writeSocketMessage(new _models_messages__WEBPACK_IMPORTED_MODULE_3__["PrivateProvidersListRequestMessage"](), callback);
    };
    PrimeSocketService.prototype.getProviderDetails = function (idHash, callback) {
        this.writeSocketMessage(new _models_messages__WEBPACK_IMPORTED_MODULE_3__["ProviderDetailsRequestMessage"](idHash), callback);
    };
    PrimeSocketService.prototype.checkProvidersKeys = function (idHash, callback) {
        var msg = new _models_messages__WEBPACK_IMPORTED_MODULE_3__["ProviderHasKeysRequestMessage"]();
        msg.id = idHash;
        this.writeSocketMessage(msg, callback);
    };
    PrimeSocketService.prototype.saveApiKeys = function (exchangeDetails, callback) {
        var msg = new _models_messages__WEBPACK_IMPORTED_MODULE_3__["ProviderSaveKeysRequestMessage"]();
        msg.id = exchangeDetails.privateApiContext.exchangeId;
        msg.key = exchangeDetails.privateApiContext.key;
        msg.secret = exchangeDetails.privateApiContext.secret;
        msg.extra = exchangeDetails.privateApiContext.extra;
        this.writeSocketMessage(msg, callback);
    };
    PrimeSocketService.prototype.deleteKeys = function (exchangeId, callback) {
        var msg = new _models_messages__WEBPACK_IMPORTED_MODULE_3__["DeleteProviderKeysRequestMessage"]();
        msg.id = exchangeId;
        this.writeSocketMessage(msg, callback);
    };
    PrimeSocketService.prototype.testPrivateApi = function (privateApiContext, callback) {
        var msg = new _models_messages__WEBPACK_IMPORTED_MODULE_3__["TestPrivateApiRequestMessage"]();
        msg.id = privateApiContext.exchangeId;
        msg.key = privateApiContext.key;
        msg.secret = privateApiContext.secret;
        msg.extra = privateApiContext.extra;
        this.writeSocketMessage(msg, callback);
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
        this.ws.onerror = this.onErrorOccurred;
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

module.exports = "\r\n<mat-toolbar color=\"primary\">\r\n  <span>{{title}}</span>\r\n</mat-toolbar>\r\n\r\n"

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
        this.title = "Prime.Manager";
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

module.exports = __webpack_require__(/*! P:\Projects\Git\prime\Ext\Prime.Manager.Client\Ui\src\main.ts */"./src/main.ts");


/***/ })

},[[0,"runtime","vendor"]]]);
//# sourceMappingURL=main.js.map
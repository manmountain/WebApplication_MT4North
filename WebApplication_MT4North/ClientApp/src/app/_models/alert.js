"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AlertType = exports.Alert = void 0;
var Alert = /** @class */ (function () {
    function Alert(init) {
        Object.assign(this, init);
    }
    return Alert;
}());
exports.Alert = Alert;
var AlertType;
(function (AlertType) {
    AlertType[AlertType["Success"] = 0] = "Success";
    AlertType[AlertType["Error"] = 1] = "Error";
    AlertType[AlertType["Info"] = 2] = "Info";
    AlertType[AlertType["Warning"] = 3] = "Warning";
})(AlertType = exports.AlertType || (exports.AlertType = {}));
//# sourceMappingURL=alert.js.map
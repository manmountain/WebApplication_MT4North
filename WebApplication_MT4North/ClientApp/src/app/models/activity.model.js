"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Activity = void 0;
var common_1 = require("./common");
var Activity = /** @class */ (function () {
    function Activity(name, description, phase, isExcluded) {
        this.name = name;
        this.description = description;
        this.phase = phase;
        this.status = common_1.Status.NOTSTARTED;
        this.isExcluded = isExcluded;
    }
    return Activity;
}());
exports.Activity = Activity;
//# sourceMappingURL=activity.model.js.map
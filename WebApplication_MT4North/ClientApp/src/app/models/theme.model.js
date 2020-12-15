"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Theme = void 0;
var Theme = /** @class */ (function () {
    function Theme(name, description) {
        this.activities = [];
        this.name = name;
        this.description = description;
    }
    Theme.prototype.addActivity = function (activity) {
        console.log("activity name in constr: ", activity.name);
        this.activities.push(activity);
    };
    return Theme;
}());
exports.Theme = Theme;
//# sourceMappingURL=theme.model.js.map
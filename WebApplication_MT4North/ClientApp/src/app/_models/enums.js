"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.UserProjectStatus = exports.ProjectRole = exports.ProjectRights = exports.ActivityStatus = exports.ActivityPhase = void 0;
var ActivityPhase;
(function (ActivityPhase) {
    ActivityPhase[ActivityPhase["CONCEPTUALIZATION"] = 0] = "CONCEPTUALIZATION";
    ActivityPhase[ActivityPhase["VALIDATION"] = 1] = "VALIDATION";
    ActivityPhase[ActivityPhase["DEVELOPMENT"] = 2] = "DEVELOPMENT";
    ActivityPhase[ActivityPhase["LAUNCH"] = 3] = "LAUNCH";
})(ActivityPhase = exports.ActivityPhase || (exports.ActivityPhase = {}));
//CONCEPTUALIZATION = "Konceptualisering",
//  VALIDATION = "Konceptvalidering",
//  DEVELOPMENT = "Produktutveckling",
//  LAUNCH = "Produktlansering"
var ActivityStatus;
(function (ActivityStatus) {
    ActivityStatus[ActivityStatus["NOTSTARTED"] = 0] = "NOTSTARTED";
    ActivityStatus[ActivityStatus["ONGOING"] = 1] = "ONGOING";
    ActivityStatus[ActivityStatus["FINISHED"] = 2] = "FINISHED";
})(ActivityStatus = exports.ActivityStatus || (exports.ActivityStatus = {}));
//NOTSTARTED = "unchecked",
//  ONGOING = "crossed",
//  FINISHED = "checked"
var ProjectRights;
(function (ProjectRights) {
    ProjectRights[ProjectRights["READ"] = 0] = "READ";
    ProjectRights[ProjectRights["WRITE"] = 1] = "WRITE";
    ProjectRights[ProjectRights["READWRITE"] = 2] = "READWRITE";
})(ProjectRights = exports.ProjectRights || (exports.ProjectRights = {}));
//READ = "R",
//  WRITE = "W",
//  READWRITE = "RW"
var ProjectRole;
(function (ProjectRole) {
    ProjectRole[ProjectRole["OWNER"] = 0] = "OWNER";
    ProjectRole[ProjectRole["PARTICIPANT"] = 1] = "PARTICIPANT";
})(ProjectRole = exports.ProjectRole || (exports.ProjectRole = {}));
//OWNER = "Projektägare",
//  PARTICIPANT = "Projektdeltagare",
var UserProjectStatus;
(function (UserProjectStatus) {
    UserProjectStatus[UserProjectStatus["PENDING"] = 0] = "PENDING";
    UserProjectStatus[UserProjectStatus["ACCEPTED"] = 1] = "ACCEPTED";
    UserProjectStatus[UserProjectStatus["REJECTED"] = 2] = "REJECTED";
})(UserProjectStatus = exports.UserProjectStatus || (exports.UserProjectStatus = {}));
//# sourceMappingURL=enums.js.map
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var registered_users_service_1 = require("./registered-users.service");
describe('RegisteredUsersService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(registered_users_service_1.RegisteredUsersService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=registered-users.service.spec.js.map
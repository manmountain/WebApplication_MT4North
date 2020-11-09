"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var connection_service_1 = require("./connection.service");
describe('ConnectionService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(connection_service_1.ConnectionService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=connection.service.spec.js.map
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var view_service_1 = require("./view.service");
describe('ViewService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(view_service_1.ViewService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=view.service.spec.js.map
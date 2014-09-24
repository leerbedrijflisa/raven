/// <reference path="typings/angularjs/angular.d.ts" />
var Raven;
(function (Raven) {
    var ValidationPage;
    (function (ValidationPage) {
        ValidationPage[ValidationPage["Input"] = 0] = "Input";
        ValidationPage[ValidationPage["Validating"] = 1] = "Validating";
        ValidationPage[ValidationPage["Output"] = 2] = "Output";
    })(ValidationPage || (ValidationPage = {}));
    ;

    var app = angular.module('raven', []);

    app.controller('ValidationController', function () {
        var _this = this;
        this.errors = validationErrors;
        this.tab = 0 /* Input */;

        this.setTab = function (tab) {
            _this.tab = tab;
        };
        this.isTabSet = function (tab) {
            return _this.tab === tab;
        };
    });

    var validationErrors = [
        {
            Message: 'The validation has been errored!',
            Line: '2',
            Column: '5'
        },
        {
            Message: 'The error has been validationed!',
            Line: '5',
            Column: '4'
        }
    ];
})(Raven || (Raven = {}));
//# sourceMappingURL=app.js.map

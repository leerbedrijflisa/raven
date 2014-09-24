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

    app.controller('ValidationController', [
        '$http',
        function ($http) {
            var _this = this;
            this.submission = submissionTemplate;
            this.errors = validationErrors;
            this.tab = 0 /* Input */;

            this.setTab = function (tab) {
                _this.tab = tab;
            };
            this.isTabSet = function (tab) {
                return _this.tab === tab;
            };
            this.submit = function () {
                $http.post("http://localhost:1262/api/v1/validator/validate", _this.submission).success(function (data, status) {
                    _this.errors = data;
                    _this.tab = 2 /* Output */;
                }).error(function (data, status) {
                    _this.errors = doubleError;
                    _this.tab = 2 /* Output */;
                });
            };
        }
    ]);

    // After this line, default and test data (no more functionality)
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
    var doubleError = [{
            Message: 'Unable to validate!',
            Line: '-1',
            Column: '-1'
        }];
    var submissionTemplate = {
        "Html": "",
        "CheckUrls": [
            "http://localhost:2746/api/check/CheckHtml"
        ]
    };
})(Raven || (Raven = {}));
//# sourceMappingURL=app.js.map

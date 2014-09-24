/// <reference path="typings/angularjs/angular.d.ts" />
var Raven;
(function (Raven) {
    'use strict';

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
            this.checkSubmission = "";
            this.errors = [];
            this.tab = 0 /* Input */;
            this.categories = [
                "Meta",
                "Security",
                "Optimization",
                "Malformed",
                "Code Style",
                "Suggestion"
            ];

            this.setTab = function (tab) {
                _this.tab = tab;
            };
            this.isTabSet = function (tab) {
                return _this.tab === tab;
            };

            this.checkKeypress = function ($event) {
                // We're only interested in enter
                if ($event.keyCode !== 13)
                    return;

                $event.preventDefault();
                _this.addCheck();
            };
            this.addCheck = function () {
                // Take the data out of the field
                var checkString = _this.checkSubmission.toLowerCase();
                _this.checkSubmission = "";

                // If the field was empty, nothing to do
                if (checkString === "")
                    return;

                var index = _this.submission.CheckUrls.indexOf(checkString);

                // If not already in the list, add
                if (index === -1)
                    _this.submission.CheckUrls.push(checkString);
            };
            this.removeCheck = function (checkString) {
                var index = _this.submission.CheckUrls.indexOf(checkString);

                // If in the list, remove
                if (index !== -1)
                    _this.submission.CheckUrls.splice(index, 1);
            };

            this.submit = function () {
                _this.setTab(1);

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
    var doubleError = [{
            Message: 'Unable to validate!',
            Line: '-1',
            Column: '-1'
        }];
    var submissionTemplate = {
        "Html": "",
        "CheckUrls": [
            "http://localhost:2746/api/check/checkhtml"
        ]
    };
})(Raven || (Raven = {}));
//# sourceMappingURL=app.js.map
